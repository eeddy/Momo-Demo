from tkinter import *
from libemg.gui import GUI
from libemg.data_handler import OnlineDataHandler, OfflineDataHandler, RegexFilter
from libemg.feature_extractor import FeatureExtractor
from libemg.emg_predictor import OnlineEMGClassifier, EMGClassifier 
from libemg.streamers import myo_streamer

class Menu:
    def __init__(self):
        # Myo Streamer - start streaming the myo data 
        streamer, sm = myo_streamer()
        self.streamer = streamer

        # Create online data handler to listen for the data
        self.odh = OnlineDataHandler(sm)

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
        Label(self.window, font=("Arial bold", 20), text = 'LibEMG - Unity Demo').pack(pady=(10,20))
        # Train Model Button
        Button(self.window, font=("Arial", 18), text = 'Train Model', command=self.launch_training).pack(pady=(0,20))
        # Play Snake Button
        Button(self.window, font=("Arial", 18), text = 'Start Classifying', command=self.start_classifying).pack()

    def start_classifying(self):
        self.window.destroy()
        self.start_classifying()

    def launch_training(self):
        self.window.destroy()
        training_ui = GUI(self.odh, gesture_height=500, gesture_width=500)
        training_ui.download_gestures([1,2,4,5], "images/")
        training_ui.start_gui()
        self.initialize_ui()

    def start_classifying(self):
        WINDOW_SIZE = 50 
        WINDOW_INCREMENT = 10

        # Step 1: Parse offline training data
        dataset_folder = 'data/'
        regex_filters = [
            RegexFilter(left_bound = "C_", right_bound="_R", values = ["0","1","2","3","4"], description='classes'),
            RegexFilter(left_bound = "R_", right_bound="_emg.csv", values = ["0", "1", "2"], description='reps'),
        ]

        odh = OfflineDataHandler()
        odh.get_data(dataset_folder, regex_filters)
        train_windows, train_metadata = odh.parse_windows(WINDOW_SIZE, WINDOW_INCREMENT)

        # Step 2: Extract features from offline data
        fe = FeatureExtractor()
        feature_list = fe.get_feature_groups()['HTD']
        training_features = fe.extract_features(feature_list, train_windows)

        # Step 3: Dataset creation
        data_set = {}
        data_set['training_features'] = training_features
        data_set['training_labels'] = train_metadata['classes']

        # Step 4: Create the EMG classifier
        o_classifier = EMGClassifier(model="LDA")
        o_classifier.fit(feature_dictionary=data_set)
        o_classifier.add_velocity(train_windows, train_metadata['classes'])

        self.window.destroy()

        # Step 5: Create online EMG classifier and start classifying.
        self.classifier = OnlineEMGClassifier(o_classifier, WINDOW_SIZE, WINDOW_INCREMENT, self.odh, feature_list)
        print('Classifier Started!')
        self.classifier.run(block=True) # block set to false so it will run in a seperate process.

    def on_closing(self):
        # Clean up all the processes that have been started
        if not self.classifier is None:
            self.classifier.stop_running()
        self.streamer.cleanup()
        self.window.destroy()

if __name__ == "__main__":
    menu = Menu()
