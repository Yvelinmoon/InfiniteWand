# -*- coding: utf-8 -*-
"""Wand trainingTest4

Automatically generated by Colab.

Original file is located at
    https://colab.research.google.com/drive/1dFjPCszvryDkZwgHAv2mBVroT0_CA44c

# Import the Library
"""

from google.colab import files


import numpy as np
import pandas as pd
import io
import tensorflow as tf
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import LSTM, Dense, Dropout
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import StandardScaler, OneHotEncoder
import matplotlib.pyplot as plt

"""# Import the dataset"""

# Upload file
uploaded = files.upload()

# Display the uploaded file name and let the user choose
print("Uploaded files:")
for filename in uploaded.keys():
    print(filename)

# Let the user input the filename to be processed

selected_file = input("Enter the filename to process: ")

"""# Read the data"""

# Read the selected file.
data = pd.read_csv(io.BytesIO(uploaded[selected_file]))
print(data.head())  # View the first few lines of data
data.describe()  # Get a statistical summary of the data

# Alternatively, use interpolation.
data.interpolate(method='linear', inplace=True)

"""# Data processing"""

# Assuming 'Label' is the classification label, and the rest are features.
X = data[['X', 'Y', 'Angle Change', 'Velocity', 'Acceleration']]
y = data['Label']

# Feature standardization
scaler = StandardScaler()
X_scaled = scaler.fit_transform(X)

# Assuming the original data is sorted in sequence, and every 50 rows represent a complete sequence of actions
# First, reshape the entire dataset
total_samples = len(data) // 50
num_features = 5  # 'X', 'Y', 'Angle Change', 'Velocity', 'Acceleration'


# Reshape X to have a shape of (number of samples, time steps, number of features)
X_reshaped = X_scaled.reshape((total_samples, 50, num_features))


# Assuming y is a Series extracted from a DataFrame
print(f'Original y type: {type(y)}')  # Check the data type of y


# Apply the same method to process labels, assuming that the labels are the same for every 50 time points
y_reshaped = y.iloc[::50]  # Take the first label from every set of 50 labels

print(f'Length of original y: {len(y)}')
print(f'Length of reshaped y: {len(y_reshaped)}')

"""# Encode the label"""

# Label encoding
encoder = OneHotEncoder(sparse_output=False)
y_encoded = encoder.fit_transform(y_reshaped.values.reshape(-1, 1))

# Check if the number of samples matches
print(f'Number of samples in X: {X_reshaped.shape[0]}')
print(f'Number of samples in y: {y_encoded.shape[0]}')

"""# Split the training and testing data"""

# If the number of samples matches, then split the dataset
if X_reshaped.shape[0] == y_encoded.shape[0]:
    X_train, X_test, y_train, y_test = train_test_split(X_reshaped, y_encoded, test_size=0.2, random_state=42)
    print("Data split successfully.")
else:
    print("Sample size mismatch.")

# Reshape the data format to fit LSTM input (samples, timesteps, features)
# Assuming each sequence has a length of 50 (based on your description)
X_train = X_train.reshape((X_train.shape[0], 50, -1))
X_test = X_test.reshape((X_test.shape[0], 50, -1))

print("Training data shape:", X_train.shape)
print("Test data shape:", X_test.shape)

"""# Construct the training model"""

model = Sequential([
    LSTM(64, input_shape=(X_train.shape[1], X_train.shape[2]), return_sequences=True),
    Dropout(0.5),
    LSTM(32),
    Dropout(0.5),
    Dense(4, activation='softmax')
])

model.compile(optimizer='adam', loss='categorical_crossentropy', metrics=['accuracy'])
model.summary()

"""# Train the model"""

history = model.fit(X_train, y_train, epochs=20, batch_size=32, validation_data=(X_test, y_test))

"""# Evaluate the model"""

# Evaluate the model on the test set
score = model.evaluate(X_test, y_test, verbose=0)
print("Test loss:", score[0])
print("Test accuracy:", score[1])

input_shape = model.input_shape
print("Model input shape:", input_shape)

"""# Draw the cruve"""

# Set the figure size
plt.figure(figsize=(10, 5))

# Plot the accuracy subplot
plt.subplot(1, 2, 1)  # The first subplot of a 1x2 grid
plt.plot(history.history['accuracy'], label='Train Accuracy')
plt.plot(history.history['val_accuracy'], label='Test Accuracy')
plt.title('Model Accuracy')
plt.xlabel('Epoch')
plt.ylabel('Accuracy')
plt.legend(loc='upper left')

# Plot the loss subplot
plt.subplot(1, 2, 2)  # The second subplot of a 1x2 grid
plt.plot(history.history['loss'], label='Train Loss')
plt.plot(history.history['val_loss'], label='Test Loss')
plt.title('Model Loss')
plt.xlabel('Epoch')
plt.ylabel('Loss')
plt.legend(loc='upper right')

# Display the plot
plt.tight_layout()
plt.show()

"""# Save the model to .h5"""

model.save('my_model.h5')  # Save the model as an HDF5 file

from google.colab import files
files.download('my_model.h5')  # Provide the file path to download the file

"""# User test"""

from IPython.display import display, HTML, Javascript
from google.colab import output
import base64
from io import BytesIO
from PIL import Image
import numpy as np
import tensorflow as tf

from google.colab import files

uploaded = files.upload()

from tensorflow.keras.models import load_model

#  Import the required libraries
import json
import numpy as np
from tensorflow.keras.models import load_model
from datetime import datetime
from google.colab import drive, output
from IPython.display import display, HTML, clear_output

# Check the input layer of the model
input_shape = model.input_shape
print("Model input shape:", input_shape)

# Load the saved model
model = load_model('6Model.h5')

# Get the last layer of the model
output_layer = model.layers[-1]

# Check the output shape of the last layer
output_shape = output_layer.output_shape

# The number of categories is typically the last dimension of the output shape
num_classes = output_shape[-1]

print("This model can recognize", num_classes, "different actions.")

# Define the feature calculation function
def scale_points(points):
    min_vals = np.min(points, axis=0)
    max_vals = np.max(points, axis=0)
    return 100 * (points - min_vals) / (max_vals - min_vals)

def calculate_angle_changes(points):
    vectors = np.diff(points, axis=0)
    angles = np.arctan2(vectors[:, 1], vectors[:, 0])
    angle_changes = np.diff(angles)
    return angle_changes

def calculate_velocities(points, times):
    distances = np.linalg.norm(np.diff(points, axis=0), axis=1)
    durations = np.diff(times)
    velocities = distances / durations
    return velocities

def calculate_accelerations(velocities, times):
    durations = np.diff(times[:-1]) # The time interval corresponding to the speed
    accelerations = np.diff(velocities) / durations
    return accelerations

def extract_features(points, times):
    points = np.array(points)
    times = np.array(times)

    if len(points) < 2:
        return np.zeros((1, 50, 5))  # Return an array filled with zeros to handle cases where there are not enough data points

    # Add: Scaling the points.
    scaled_points = scale_points(points)

    angle_changes = calculate_angle_changes(scaled_points)  # Use the scaled points
    velocities = calculate_velocities(scaled_points, times)
    accelerations = calculate_accelerations(velocities, times)

    # Feature combination
    num_features = min(len(angle_changes), len(velocities), len(accelerations))
    features = np.column_stack([
        scaled_points[-num_features-1:-1],  # Trim the point data to the corresponding length
        angle_changes[:num_features],
        velocities[:num_features],
        accelerations[:num_features]
    ])

    # Ensure there are 50 time steps
    if features.shape[0] < 50:
        padding = np.zeros((50 - features.shape[0], 5))
        features = np.vstack([features, padding])
    elif features.shape[0] > 50:
        features = features[:50]

    return features.reshape(1, 50, 5)

# Assuming the model and labels are already defined
action_labels = {0: 'Circle', 1: 'Curve', 2: 'Triangle', 3: 'Else'}

def predict_action(features):
    prediction = model.predict(features)
    predicted_action_code = np.argmax(prediction)
    predicted_action_label = action_labels[predicted_action_code]
    return predicted_action_code, predicted_action_label

def process_data(points_json, times_json):
    points = json.loads(points_json)
    times = json.loads(times_json)

    if not points:
        print("No points received.")
        return

    features = extract_features(points, times)
    action_code, action_label = predict_action(features)

    # Call the frontend function to display the results
    display(Javascript(f"displayResult({action_code}, '{action_label}', {features.tolist()}, {features.shape})"))

output.register_callback('notebook.process_data', process_data)

# Front-end HTML/JavaScript code
html_code = """
<div>
    <canvas id="canvas" width="800" height="600" style="border:1px solid black;"></canvas>
</div>
<div id="output"></div> <!-- Display predicted results -->
<div id="featureData"></div> <!-- Display feature data and shape -->

<script>
var canvas = document.getElementById('canvas');
var ctx = canvas.getContext('2d');
var points = [];
var times = [];
var maxPoints = 50; // Maximum number of points is 50
var drawing = false;

canvas.addEventListener('mousedown', function(event) {
    drawing = true;
    points = []; // Reset points array
    times = []; // Reset times array
    ctx.clearRect(0, 0, canvas.width, canvas.height); // Clear canvas
    clearOutput(); // Clear output results
    clearFeatureData(); // Clear feature data display
});

canvas.addEventListener('mousemove', function(event) {
    if (drawing && points.length < maxPoints) {
        var rect = canvas.getBoundingClientRect();
        var x = event.clientX - rect.left;
        var y = event.clientY - rect.top;
        var time = new Date().getTime();
        points.push([x, y]);
        times.push(time);
        ctx.fillStyle = 'rgb(255,0,0)';
        ctx.fillRect(x, y, 2, 2); // Draw point
    }
});

canvas.addEventListener('mouseup', function(event) {
    drawing = false;
    if (points.length > 0) {
        sendData();
    }
});

function sendData() {
    try {
        google.colab.kernel.invokeFunction('notebook.process_data', [JSON.stringify(points), JSON.stringify(times)], {});
    } catch (error) {
        console.error("Failed to send data: " + error);
    }
}

function displayResult(prediction, label, features, shape) {
    var outputDiv = document.getElementById('output');
    outputDiv.innerHTML = 'Predicted Action: ' + label + ' (Code: ' + prediction + ')';
    var featureDataDiv = document.getElementById('featureData');
    featureDataDiv.innerHTML = 'Feature Data: ' + JSON.stringify(features) + '<br>Shape: ' + JSON.stringify(shape);
}

function clearOutput() {
    var outputDiv = document.getElementById('output');
    outputDiv.innerHTML = ''; // Clear output area
}

function clearFeatureData() {
    var featureDataDiv = document.getElementById('featureData');
    featureDataDiv.innerHTML = ''; // Clear feature data display area
}
</script>
"""

# Display HTML code
display(HTML(html_code))

"""# Save as tflite"""

converter = tf.lite.TFLiteConverter.from_keras_model(model)

# Enable Select TensorFlow operations
converter.target_spec.supported_ops = [
    tf.lite.OpsSet.TFLITE_BUILTINS,  # Enable TensorFlow Lite ops
    tf.lite.OpsSet.SELECT_TF_OPS     # Enable Select TF ops
]

# Disable degradation of TensorList operations
converter._experimental_lower_tensor_list_ops = False

# Try to convert the model
try:
    tflite_model = converter.convert()
    # Save the converted model to a file
    tflite_model_file = 'model.tflite'
    with open(tflite_model_file, 'wb') as f:
        f.write(tflite_model)
    print("Model conversion successful, preparing to download file.")

    # Download the file to the local machine
    files.download(tflite_model_file)

except Exception as e:
    print("Model conversion failed:", e)

!pip install tensorflow tf2onnx

import tensorflow as tf

model = load_model('6Model.h5')


import tf2onnx

# Set the input shape and type, assuming input type is float32
spec = (tf.TensorSpec((None, 50, 5), tf.float32, name="input"),)

# Convert the model, specify ONNX version as opset 13
onnx_model, _ = tf2onnx.convert.from_keras(model, input_signature=spec, opset=13)

# Save the ONNX model to a file
with open("model.onnx", "wb") as f:
    f.write(onnx_model.SerializeToString())

from google.colab import files

uploaded = files.upload()
for fn in uploaded.keys():
  print('User uploaded file "{name}" with length {length} bytes'.format(
      name=fn, length=len(uploaded[fn])))

!pip install onnx

import onnx

# Assume the uploaded file name is model.onnx, make sure to replace it with the actual uploaded file name
model_path = list(uploaded.keys())[0]  # Take the first file name from the uploaded file list

onnx_model = onnx.load(model_path)

# Print the input and output information of the model
print("Model input information:")
for input_tensor in onnx_model.graph.input:
    print(input_tensor.name, end=': ')
    # Get the shape and data type
    shape = [dim.dim_value if dim.dim_value > 0 else None for dim in input_tensor.type.tensor_type.shape.dim]
    data_type = input_tensor.type.tensor_type.elem_type
    print("Shape =", shape, "Data Type =", onnx.TensorProto.DataType.Name(data_type))

print("\nModel output information:")
for output_tensor in onnx_model.graph.output:
    print(output_tensor.name, end=': ')
    shape = [dim.dim_value if dim.dim_value > 0 else None for dim in output_tensor.type.tensor_type.shape.dim]
    data_type = output_tensor.type.tensor_type.elem_type
    print("Shape =", shape, "Data Type =", onnx.TensorProto.DataType.Name(data_type))