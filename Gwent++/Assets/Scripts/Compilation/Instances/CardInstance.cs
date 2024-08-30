using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using LogicalSide;

namespace Compiler
{
    public class CardInstance : Expression
    {

        public Expression? Name { get; set; }
        public Expression? Type { get; set; }
        public Expression? Faction { get; set; }
        public Expression? Power { get; set; }
        public List<Expression>? Range { get; set; }
        public OnActivation? OnActivation { get; set; }

        public override ValueType? CheckSemantic(Scope scope)
        {
            printed = "Card";
            SemScope = new Scope(scope);
            if (Name != null)
                Name.CheckSemantic(scope);
            else
            {
                Errors.List.Add(new CompilingError("Card must have a name", new Position()));
            }
            if (Type != null)
                Type.CheckSemantic(scope);
            else
            {
                Errors.List.Add(new CompilingError("Card must have a type", new Position()));
            }
            if (Faction != null)
                Faction.CheckSemantic(scope);
            else
            {
                Errors.List.Add(new CompilingError("Card must have a faction", new Position()));
            }
            //this property can be optional
            if (Power != null && Power.CheckSemantic(scope)!= ValueType.Int)
                Errors.List.Add(new CompilingError("Card Power need to be evaluated as an integer", new Position()));

            //this property can be optional
            if (Range != null && Range.Count != 0)
                foreach (var range in Range)
                {
                    range.CheckSemantic(scope);
                }

            if (OnActivation != null)
                OnActivation.CheckSemantic(scope);
            return ValueType.Checked;
        }

        readonly Dictionary<string, string> Mapping = new()
    {
        {"Melee","M"},
        {"Range","R"},
        {"Siege","S"}
    };

        public override object Evaluate(Scope scope, object set, object instance = null)
        {
            string range = "";
            string temp;
            if(Range != null && Range.Count != 0)
            foreach (Expression item in Range)
            {
                temp = (string)item.Evaluate(scope, set);
                if (Mapping.ContainsKey(temp))
                {
                    range += Mapping[temp];
                }
                else
                {
                    Errors.List.Add(new CompilingError("Range must be valid", new Position()));
                }
            }

            int Pow = 0;
            if(Power!= null)
            {
                Pow = (int)Power.Evaluate(scope, set);
            }
            List<IEffect> effects = null;
            if (OnActivation != null)
            {
                effects = (List<IEffect>)OnActivation!.Evaluate(scope, null!);
            }

            CompilerCard card = new((string)Name!.Evaluate(scope, set), (string)Type!.Evaluate(scope, set),
            Pow, range, new Player(), (string)Faction!.Evaluate(scope, set),
            effects);
            return card;
        }
    }

    public class OnActivation : Expression
    {
        public List<EffectParam>? Effects = new();

        public override ValueType? CheckSemantic(Scope scope)
        {
            printed = "OnActivation";
            if (Effects != null)
                foreach (var effect in Effects)
                {
                    effect.CheckSemantic(SemScope);
                }
            return ValueType.Checked;
        }
        public override object Evaluate(Scope scope, object set, object instance = null)
        {
            List<IEffect> effects = new();
        if (Effects != null)
        foreach (EffectParam assignment in Effects)
        {
            assignment.Evaluate(scope, set, effects);
        }
            Result = effects;
            return Result;
        }
        public override void Print(int indentLevel = 0)
        {
            printed = "OnActivacion";
            Console.WriteLine(new string(' ', indentLevel * 4) + printed);
        }
    }
    public class EffectParam : Expression
    {
        public List<Expression>? Effect = new();
        public Selector? Selector;
        public EffectParam? PostAction;

        public override ValueType? CheckSemantic(Scope scope)
        {
            SemScope = new Scope(scope);
            SemScope.WithoutReps = true;
            if (Effect != null)
                foreach (var effect in Effect)
                {
                    effect.CheckSemantic(SemScope);
                }
            else
            {
                Errors.List.Add(new CompilingError("Effect must have a name", new Position()));
            }
            SemScope.WithoutReps = false;
            if (Selector != null && Selector.CheckSemantic(SemScope) != ValueType.Checked)
            {
                Errors.List.Add(new CompilingError("Effect must have a selector", new Position()));
            }

            if (PostAction != null)
            {
                if (!(PostAction.CheckSemantic(scope) == ValueType.Checked))
                {
                    Errors.List.Add(new CompilingError("Effect must have a valid post action", new Position()));
                }
            }
            return ValueType.Checked;
        }

        public override object Evaluate(Scope scope, object set, object instance = null)
        {
            List<IdentifierExpression> CorrectParams = new();
            for (int i = 0; i < Effect.Count; i++)
            {
                Expression exp = Effect[i];
                object value = exp.Evaluate(scope, null);
                if (exp is BinaryExpression bin)
                {
                    bin.Left.Result = value;
                    exp = bin.Left;
                    CorrectParams.Add((IdentifierExpression)exp);
                }
            }
            Result = Processor.FindEffect(CorrectParams);

            Result = new MyEffect((EffectInstance)Result, Selector, CorrectParams);
            if (instance is List<IEffect> list)
            {
                list.Add((IEffect)Result);
            }
            if (Selector != null)
                Selector.Evaluate(scope, set, instance);
            if (PostAction != null)
            {
                if (PostAction.Selector != null)
                    PostAction.Evaluate(scope, Selector, instance);
                else{
                    PostAction.Selector = Selector;
                    PostAction.Evaluate(scope, set, instance);
                }
            }
            return null;
        }
        public override void Print(int indentLevel = 0)
        {
            printed = "OnActivacion";
            Console.WriteLine(new string(' ', indentLevel * 4) + printed);
        }
    }
    public class Selector : Expression
    {
        public Expression? Source;
        public Expression? Single;
        public Expression? Predicate;

        public Selector Parent;

        public override ValueType? CheckSemantic(Scope scope)
        {
            SemScope = new Scope(scope);
            if (Source != null && Source.CheckSemantic(scope) == ValueType.String)
            {
                Source.CheckType = ValueType.String;
            }
            else
            {
                Errors.List.Add(new CompilingError("Selector must have a source", new Position()));
            }
            if (Single != null && Single.CheckSemantic(scope) == ValueType.Bool)
            {
                Single.CheckType = ValueType.Bool;
            }
            else
            {
                Errors.List.Add(new CompilingError("Selector must have a single", new Position()));
            }
            if (Predicate != null && Predicate.CheckSemantic(scope) != ValueType.Predicate)
                Errors.List.Add(new CompilingError("Selector must have a predicate", new Position()));

            return ValueType.Checked;
        }

        readonly Dictionary<string, string> Mapping = new()
        {
            {"hand", "Hand"},
            {"otherhand", "OtherHand"},
            {"deck", "Deck"},
            {"otherdeck", "OtherDeck"},
            {"field", "Field"},
            {"graveyard", "GraveYard"},
            {"board", "Board"},
            {"otherfield", "Otherfield"},
            {"othergraveyard", "OtherGraveYard"},
            {"parent","parent"}
        };

        public override object? Evaluate(Scope scope, object set, object instance = null)
        {
            string s = (string)Source!.Evaluate(scope, set, instance);
            if (Mapping.ContainsKey(s))
            {
                if (s == "parent")
                {
                    if(set!= null)
                        Parent = (Selector)set;
                    else
                        Errors.List.Add(new CompilingError("Evaluate Error, use of parent source after an Empty Selector, or in a non PostAction Statement", new Position()));
                }
                Single!.Result = Single.Evaluate(scope, null!, null!);
            }
            else
                Errors.List.Add(new CompilingError("Selector must have a valid source", new Position()));

            return null;
        }

        public List<UnityCard> Execute(IDeckContext context)
        {
            Predicate predicate = (Predicate as Predicate)!;

            List<UnityCard> SourceCards;
            var cont = context.GetType();
            if(Parent == null)
                SourceCards = (List<UnityCard>)cont.GetProperty(Mapping[(string)Source.Result!]).GetValue(context);
            else
                SourceCards= Parent.Execute(context);
            List<UnityCard> Targets = new();

            foreach (UnityCard card in SourceCards)
            {
                if ((bool)predicate.Evaluate(null!, card))
                {
                    Targets.Add(card);
                    if ((bool)Single!.Result!)
                        break;
                }
            }
            return Targets;
        }
        public override void Print(int indentLevel = 0)
        {
            printed = "Selector";
            Console.WriteLine(new string(' ', indentLevel * 4) + printed);
        }
    }

    public class Predicate : Expression
    {
        public IdentifierExpression? Unit;
        public Expression? Condition;
        public override ValueType? CheckSemantic(Scope scope)
        {
            SemScope = new Scope(scope);
            if (Unit != null)
            {
                ValueType? type;
                object v;
                if (!scope.Find(Unit, out type, out v))
                {
                    Unit.CheckType = ValueType.Card;
                    SemScope.AddVar(Unit);
                }
            }
            else
            {
                Errors.List.Add(new CompilingError("Predicate most have a unit or unit already declarated", new Position()));
            }
            if (Condition != null && Condition.CheckSemantic(SemScope) == ValueType.Bool)
            {
                Condition.CheckType = ValueType.Bool;
            }
            else
            {
                Errors.List.Add(new CompilingError("Predicate must have a valid condition", new Position()));
            }
            return ValueType.Predicate;

        }
        public override object Evaluate(Scope scope, object set, object instance = null)
        {
            Scope Evaluator = new Scope(scope);
            Unit!.Result = set;
            Evaluator.AddVar(Unit);
            return Condition!.Evaluate(Evaluator, null!);
        }
    }

    public abstract class Card: ScriptableObject
    {
        public abstract string Name { get; set; }
        public abstract string Type { get; set; }
        public abstract int Power { get; set; }
        public abstract string Range { get; set; }
        public abstract Player Owner { get; set; }
        public abstract string Faction { get; set; }
        public abstract List<IEffect> Effects { get; set; }
        public abstract Card GetCopy();

        public void Execute(IDeckContext context)
        {
            foreach (IEffect effect in Effects)
            {
                effect.Execute(context);
            }
        }
    }

    public class CompilerCard : Card
    {
        public override string Name { get; set; }
        public override string Type { get; set; }
        public override int Power { get; set; }
        public override string Range { get; set; }
        public override Player Owner { get; set; }

        public override string Faction { get; set; }
        public override List<IEffect> Effects { get; set; }
        public CompilerCard(string name, string type, int power, string range, Player owner, string faction, List<IEffect> effects)
        {
            Name = name;
            Type = type;
            Power = power;
            Range = range;
            Owner = owner;
            Faction = faction;
            Effects = effects;
        }
        public override Card GetCopy()
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            string result = "";
            result += "Name: " + Name + "\n";
            result += "Type: " + Type + "\n";
            result += "Power: " + Power + "\n";
            result += "Range: " + Range + "\n";
            result += "Owner: " + Owner + "\n";
            result += "Faction: " + Faction + "\n";
            result += "Efectos: \n";
            int conta = 1;
            foreach (IEffect effect in Effects)
            {
                Console.WriteLine($"{conta++}- " + effect);
            }
            return result;
        }
    }

    public class Player
    {
        public bool Turn { get; set; }
    }

    public interface IDeckContext
    {
        bool Turn { get; }
        public List<UnityCard> Deck { get; }
        public List<UnityCard> OtherDeck { get; }
        public List<UnityCard> DeckOfPlayer(Player player);


        public List<UnityCard> GraveYard { get; }
        public List<UnityCard> OtherGraveYard { get; }

        public List<UnityCard> GraveYardOfPlayer(Player player);

        public List<UnityCard> Field { get; }
        public List<UnityCard> OtherField { get; }
        public List<UnityCard> FieldOfPlayer(Player player);


        public List<UnityCard> Hand { get; }
        public List<UnityCard> OtherHand { get; }


        public List<UnityCard> HandOfPlayer(Player player);
        public List<UnityCard> Board { get; }
        public Player TriggerPlayer { get; }
    }
}