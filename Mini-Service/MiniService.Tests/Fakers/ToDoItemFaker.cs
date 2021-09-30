using AutoBogus;
using Messaging.MiniService.Enums;
using MiniService.Data.Domain.Entities;

namespace MiniService.Tests.Fakers
{
    public class ToDoItemNewFaker : AutoFaker<TodoItem>
    {
        public ToDoItemNewFaker() : base()
        {
            RuleFor(o => o.Title, f => f.Lorem.Sentence(1, 5));
            RuleFor(o => o.Priority, f => f.PickRandomWithout(PriorityLevel.None));
        }
    }

    public class ToDoItemExistingFaker : ToDoItemNewFaker
    {
        public ToDoItemExistingFaker() : base()
        {
            RuleFor(o => o.Id, f => f.Random.Int());
            RuleFor(o => o.Created, f => f.Date.Recent());
            RuleFor(o => o.Done, f => false);
            RuleFor(o => o.Updated, (f, i) => i.Created);
        }
    }

    public sealed class ToDoItemDoneFaker : ToDoItemExistingFaker
    {
        public ToDoItemDoneFaker() : base()
        {
            RuleFor(o => o.Done, f => true);
        }
    }
}
