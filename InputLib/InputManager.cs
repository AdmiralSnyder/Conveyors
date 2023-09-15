namespace InputLib;

public class InputManager
{
    private Queue<InputStage> Stages = new();

    private Queue<InputStage> DoneStages = new();

    public static BlankInputEntry Blank()
    {
        var manager = new InputManager();
        var blankEntry = new BlankInputEntry(manager);
        return blankEntry;
    }

    public void AddStage<TInput, T>(Func<TInput, Task<InputResult<T>>> stageFunc, string? name = null)
    {
        Stages.Enqueue(new InputStage<TInput, T>() { StageFunc = stageFunc, Name = name });
    }

    internal async Task<InputResult<T>> Run<T>()
    {
        InputStage lastStage = InputStage.Initial;
        while (true)
        {
            if (Stages.Any())
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
            else break;
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
