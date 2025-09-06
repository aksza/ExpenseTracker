namespace ExpenseTracker.Validators
{
    public class ValidationResult
    {
        public bool IsValid => !Errors.Any();
        public List<string> Errors { get; } = new();
        public string ErrorMessage => string.Join(", ", Errors);

        public void AddError(string error) => Errors.Add(error);
    }
}
