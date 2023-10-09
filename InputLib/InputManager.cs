namespace InputLib;

public class InputManager
{
    private Queue<InputStage> Stages = new();

    private Queue<InputStage> DoneStages = new();

    public static BlankInputEntry BlankContext()
    {
        var manager = new InputManager();
        var blankEntry = new BlankInputEntry(manager);
        return blankEntry;
    }

    public void AddStage<TInput, T>(Func<TInput, Task<InputResult<T>>> stageFunc, string? name = null, bool skipNesting = false)
    {
        Stages.Enqueue(new InputStage<TInput, T>() { StageFunc = stageFunc, Name = name, SkipNesting = skipNesting });
    }

    internal async Task<InputResult<T>> Run<T>()
    {
        InputStage lastStage = InputStage.Initial;
        while (Stages.Any())
        {
            var stage = Stages.Dequeue();
            await stage.Invoke(lastStage);
            if (stage.IsSuccess())
            {
                DoneStages.Enqueue(stage);
                lastStage = stage;
            }
            else
            {
                // TODO else retry depending on strategy
                return InputResult.Failure;
            }
        }

        if (lastStage is InputStage<T> lastStageT)
        {
            return lastStageT.Output;
        }
        else
        {
            return InputResult.Failure;
        }
    }
}
