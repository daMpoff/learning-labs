namespace OptimalStrategy;

public interface ICriterion
{
    (double value, int strategy) FindOptimalStrategy();
}