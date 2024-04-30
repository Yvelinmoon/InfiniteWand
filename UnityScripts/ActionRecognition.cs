using UnityEngine;
using Unity.Barracuda;
using System.Collections.Generic;
using System.Linq;

public class ActionRecognition : MonoBehaviour
{
    public NNModel modelAsset;
    private IWorker worker;

    public static ActionRecognition Instance;
    public ActionTrigger actionTrigger;  

    private void Awake()
    {
        Instance = this;
        var model = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
    }

    
    private Tensor PreprocessData(List<Vector2> points, List<float> times)
    {
        // Scale points to a fixed size
        List<Vector2> scaledPoints = ScalePoints(points, new Vector2(100, 100));

        // Calculate angle changes
        List<float> angleChanges = CalculateAngleChanges(scaledPoints);

        // Calculate velocities
        List<float> velocities = CalculateVelocities(scaledPoints, times);

        // Calculate accelerations
        List<float> accelerations = CalculateAccelerations(velocities, times);

        // Integrate all data into a matrix of shape (50, 5)
        float[] flatData = new float[50 * 5];
        for (int i = 0; i < 50; i++)
        {
            int idx = i * 5;
            if (i < scaledPoints.Count)
            {
                flatData[idx] = scaledPoints[i].x;
                flatData[idx + 1] = scaledPoints[i].y;
            }
            if (i < angleChanges.Count) flatData[idx + 2] = angleChanges[i];
            if (i < velocities.Count) flatData[idx + 3] = velocities[i];
            if (i < accelerations.Count) flatData[idx + 4] = accelerations[i];
        }

        // Create a Tensor from flattened data
        Tensor tensor = new Tensor(new TensorShape(4,1,5,5), flatData);
        Debug.Log($"Tensor shape before model input: {tensor.shape}");



        return tensor;
    }

    // Implement other methods (ScalePoints, CalculateAngleChanges, CalculateVelocities, CalculateAccelerations) as shown previously

    public void ProcessPoints(List<Vector2> rawPoints, List<float> times)
    {
        Tensor inputTensor = PreprocessData(rawPoints, times);
        Debug.Log($"Tensor shape after preprocessing: {inputTensor.shape}");
        PredictAction(inputTensor);
    }





    private List<Vector2> ScalePoints(List<Vector2> points, Vector2 targetSize)
    {
        float minX = points.Min(p => p.x);
        float maxX = points.Max(p => p.x);
        float minY = points.Min(p => p.y);
        float maxY = points.Max(p => p.y);

        if (maxX == minX || maxY == minY) return new List<Vector2>(points);

        float widthScale = targetSize.x / (maxX - minX);
        float heightScale = targetSize.y / (maxY - minY);

        return points.Select(p => new Vector2((p.x - minX) * widthScale, (p.y - minY) * heightScale)).ToList();
    }

    private List<float> CalculateAngleChanges(List<Vector2> points)
    {
        List<float> angles = new List<float>();
        for (int i = 1; i < points.Count; i++)
        {
            Vector2 diff = points[i] - points[i - 1];
            angles.Add(Mathf.Atan2(diff.y, diff.x));
        }

        List<float> angleChanges = new List<float>();
        for (int i = 1; i < angles.Count; i++)
        {
            angleChanges.Add(angles[i] - angles[i - 1]);
        }

        return angleChanges;
    }

    private List<float> CalculateVelocities(List<Vector2> points, List<float> times)
    {
        List<float> velocities = new List<float>();
        for (int i = 1; i < points.Count; i++)
        {
            float distance = Vector2.Distance(points[i], points[i - 1]);
            float timeDelta = times[i] - times[i - 1];
            if (timeDelta == 0) timeDelta = 1e-10f; // Avoid division by zero
            velocities.Add(distance / timeDelta);
        }
        return velocities;
    }

    private List<float> CalculateAccelerations(List<float> velocities, List<float> times)
    {
        List<float> accelerations = new List<float>();
        for (int i = 1; i < velocities.Count; i++)
        {
            float velocityChange = velocities[i] - velocities[i - 1];
            float timeDelta = times[i + 1] - times[i];
            if  (timeDelta == 0) timeDelta = 1e-10f; // Avoid division by zero
                accelerations.Add(velocityChange / timeDelta);
        }
        return accelerations;
    }

    private void PredictAction(Tensor inputTensor)
    {
        worker.Execute(inputTensor);
        Tensor output = worker.PeekOutput();
        InterpretResult(output);
        inputTensor.Dispose();
        output.Dispose();
    }

    private void InterpretResult(Tensor output)
    {
        // Assuming the output is a 1D Tensor of probabilities for each class
        float[] probabilities = output.ToReadOnlyArray();
        int maxIndex = 0;
        float maxProb = probabilities[0];
        for (int i = 1; i < probabilities.Length; i++)
        {
            if (probabilities[i] > maxProb)
            {
                maxIndex = i;
                maxProb = probabilities[i];
            }
        }

        Debug.Log($"Predicted class index: {maxIndex} with probability: {maxProb}");
        
        // 使用 actionTrigger 触发动作
        actionTrigger.HandlePredictedIndex(maxIndex);
    }

    private void OnDestroy()
    {
        worker.Dispose();
    }
}