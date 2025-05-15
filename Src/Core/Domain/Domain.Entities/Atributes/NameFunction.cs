namespace Domain.Entities.Atributes;

public class NameFunction : Attribute
{
    public string Name { get; set; }

    public NameFunction(string name)
    {
        Name = name;
    }
}