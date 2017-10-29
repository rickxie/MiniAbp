namespace MiniAbp.Domain.Entities
{
    public class IdInput<T> : IInputDto
    {
        public T Id { get; set; }
    }
}
