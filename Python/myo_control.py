import socket
import multiprocessing
from tkinter import *
from pyomyo import Myo, emg_mode
from unb_emg_toolbox.training_ui import TrainingUI
from unb_emg_toolbox.data_handler import OnlineDataHandler, OfflineDataHandler
from unb_emg_toolbox.utils import make_regex
from unb_emg_toolbox.feature_extractor import FeatureExtractor
from unb_emg_toolbox.emg_classifier import OnlineEMGClassifier

def stream_myo():
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    m = Myo(mode=emg_mode.FILTERED)
    m.connect()

    def write_to_socket(emg, movement):
        sock.sendto(bytes(str(emg), "utf-8"), ('127.0.0.1', 12345))
    m.add_emg_handler(write_to_socket)

    m.vibrate(1)

    while True:
        try:
            m.run()
        except:
            print("Worker Stopped")
            quit() 

class Menu:
    def __init__(self):
        # Myo Streamer - start streaming the myo data 
        self.myo = multiprocessing.Process(target=stream_myo, daemon=True)
        self.myo.start()

        # Create online data handler to listen for the data
        self.odh = OnlineDataHandler(emg_arr=True)
        self.odh.get_data()

        self.classifier = None

        self.window = None
        self.initialize_ui()
        self.window.mainloop()

    def initialize_ui(self):
        # Create the simple menu UI:
        self.window = Tk()
        self.window.protocol("WM_DELETE_WINDOW", self.on_closing)
        self.window.title("Game Menu")
        self.window.geometry("500x200")

        # Label 
        Label(self.window, font=("Arial bold", 20), text = 'UNB EMG Toolbox - Unity Demo').pack(pady=(10,20))
        # Train Model Button
        Button(self.window, font=("Arial", 18), text = 'Get Training Data', command=self.launch_training).pack(pady=(0,20))
        # Play Snake Button
        Button(self.window, font=("Arial", 18), text = 'Start Classifying', command=self.start_classifying).pack()

    def start_classifying(self):
        self.window.destroy()
        self.start_classifying()

    def launch_training(self):
        self.window.destroy()
        # Launch training ui
        TrainingUI(num_reps=3, rep_time=3, rep_folder="Class_Images/", output_folder="data/", data_handler=self.odh)
        self.initialize_ui()

    def start_classifying(self):
        WINDOW_SIZE = 50 
        WINDOW_INCREMENT = 10

        # Step 1: Parse offline training data
        dataset_folder = 'data/'
        classes_values = ["0","1","2","3"]
        classes_regex = make_regex(left_bound = "_C_", right_bound=".csv", values = classes_values)
        reps_values = ["0", "1", "2"]
        reps_regex = make_regex(left_bound = "R_", right_bound="_C_", values = reps_values)
        dic = {
            "reps": reps_values,
            "reps_regex": reps_regex,
            "classes": classes_values,
            "classes_regex": classes_regex
        }

        odh = OfflineDataHandler()
        odh.get_data(folder_location=dataset_folder, filename_dic=dic, delimiter=",")
        train_windows, train_metadata = odh.parse_windows(WINDOW_SIZE, WINDOW_INCREMENT)

        # Step 2: Extract features from offline data
        fe = FeatureExtractor(num_channels=8)
        feature_list = fe.get_feature_groups()['LS9']
        training_features = fe.extract_features(feature_list, train_windows)

        # Step 3: Dataset creation
        data_set = {}
        data_set['training_features'] = training_features
        data_set['training_labels'] = train_metadata['classes']
        data_set['training_windows'] = train_windows

        # Step 4: Create online EMG classifier and start classifying.
        self.classifier = OnlineEMGClassifier(model="SVM", data_set=data_set, num_channels=8, window_size=WINDOW_SIZE, window_increment=WINDOW_INCREMENT, 
                online_data_handler=self.odh, features=feature_list, rejection_type="CONFIDENCE", rejection_threshold=0.75, majority_vote=10, velocity=True, std_out=True)
        self.classifier.run(block=False) # block set to false so it will run in a seperate process.

    def on_closing(self):
        # Clean up all the processes that have been started
        if not self.classifier is None:
            self.classifier.stop_running()
        self.myo.terminate()
        self.odh.stop_data()
        self.window.destroy()

if __name__ == "__main__":
    menu = Menu()