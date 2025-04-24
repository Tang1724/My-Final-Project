using NUnit.Framework;

public class MathLogicTests
{
    [Test]
    public void DamageCalculation_IsCorrect()
    {
        int baseDamage = 10;
        float multiplier = 1.5f;
        int expected = 15;

        int result = (int)(baseDamage * multiplier);

        Assert.AreEqual(expected, result);
    }
}