namespace JobBoard;

public record ValidationError(string Field, List<string> Messages);
