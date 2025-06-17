using System.ComponentModel.DataAnnotations;

namespace Pipe.Infrastructure.Models
{
	public abstract class EntityBaseWithTypedId<TId> : ValidatableObject, IEntityWithTypedId<TId>
	{
		public virtual TId Id { get; protected set; }
	}
}
