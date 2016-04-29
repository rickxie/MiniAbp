namespace MiniAbp.Domain.Entitys
{
    public class IdInput<T> : IInputDto
    {
        public T Id { get; set; }
    }
}
