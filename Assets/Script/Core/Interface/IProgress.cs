
interface IProgress
{
    void Assign();
    void Initialize();
    void Progress(float deltaTime);
    void AfterProgress(float deltaTime);
    void Release();
}