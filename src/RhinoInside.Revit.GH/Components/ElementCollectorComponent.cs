using System.Collections.Generic;
using System.Linq;
using DB = Autodesk.Revit.DB;

namespace RhinoInside.Revit.GH.Components
{
  public abstract class ElementCollectorComponent : ZuiComponent
  {
    protected ElementCollectorComponent(string name, string nickname, string description, string category, string subCategory)
    : base(name, nickname, description, category, subCategory) { }

    protected virtual DB.ElementFilter ElementFilter { get; } = default;
    public override bool NeedsToBeExpired
    (
      DB.Document document,
      ICollection<DB.ElementId> added,
      ICollection<DB.ElementId> deleted,
      ICollection<DB.ElementId> modified
    )
    {
      var elementFilter = ElementFilter;
      var _Filter_ = Params.IndexOfInputParam("Filter");
      var filters = _Filter_ < 0 ?
                    Enumerable.Empty<DB.ElementFilter>() :
                    Params.Input[_Filter_].VolatileData.AllData(true).
                    OfType<Types.ElementFilter>().
                    Select(x => new DB.LogicalAndFilter(elementFilter, x.Value));

      foreach (var filter in filters.Any() ? filters : Enumerable.Repeat(elementFilter, 1))
      {
        if (added.Where(x => filter?.PassesFilter(document, x) ?? true).Any())
          return true;

        if (modified.Where(x => filter?.PassesFilter(document, x) ?? true).Any())
          return true;

        if (deleted.Count > 0)
        {
          var empty = new DB.ElementId[0];
          foreach (var param in Params.Output.OfType<Kernel.IGH_ElementIdParam>())
          {
            if (param.NeedsToBeExpired(document, empty, deleted, empty))
              return true;
          }
        }
      }

      return false;
    }

    protected static bool TryGetFilterIntegerParam(DB.BuiltInParameter paramId, int pattern, out DB.ElementFilter filter)
    {
      var rule = new DB.FilterIntegerRule
      (
        new DB.ParameterValueProvider(new DB.ElementId(paramId)),
        new DB.FilterNumericEquals(),
        pattern
      );

      filter = new DB.ElementParameterFilter(rule, false);
      return true;
    }

    protected static bool TryGetFilterDoubleParam(DB.BuiltInParameter paramId, double pattern, out DB.ElementFilter filter)
    {
      var rule = new DB.FilterDoubleRule
      (
        new DB.ParameterValueProvider(new DB.ElementId(paramId)),
        new DB.FilterNumericEquals(),
        pattern,
        1e-6
      );

      filter = new DB.ElementParameterFilter(rule, false);
      return true;
    }

    protected static bool TryGetFilterDoubleParam(DB.BuiltInParameter paramId, double pattern, double tolerance, out DB.ElementFilter filter)
    {
      var rule = new DB.FilterDoubleRule
      (
        new DB.ParameterValueProvider(new DB.ElementId(paramId)),
        new DB.FilterNumericEquals(),
        pattern,
        tolerance
      );

      filter = new DB.ElementParameterFilter(rule, false);
      return true;
    }

    protected static bool TryGetFilterLengthParam(DB.BuiltInParameter paramId, double pattern, out DB.ElementFilter filter)
    {
      var rule = new DB.FilterDoubleRule
      (
        new DB.ParameterValueProvider(new DB.ElementId(paramId)),
        new DB.FilterNumericEquals(),
        pattern,
        Revit.VertexTolerance
      );

      filter = new DB.ElementParameterFilter(rule, false);
      return true;
    }

    protected internal static bool TryGetFilterStringParam(DB.BuiltInParameter paramId, ref string pattern, out DB.ElementFilter filter)
    {
      if (pattern is string subPattern)
      {
        var inverted = false;
        var method = Operator.CompareMethodFromPattern(ref subPattern, ref inverted);
        if (Operator.CompareMethod.Nothing < method && method < Operator.CompareMethod.Wildcard)
        {
          var evaluator = default(DB.FilterStringRuleEvaluator);
          switch (method)
          {
            case Operator.CompareMethod.Equals: evaluator = new DB.FilterStringEquals(); break;
            case Operator.CompareMethod.StartsWith: evaluator = new DB.FilterStringBeginsWith(); break;
            case Operator.CompareMethod.EndsWith: evaluator = new DB.FilterStringEndsWith(); break;
            case Operator.CompareMethod.Contains: evaluator = new DB.FilterStringContains(); break;
          }

          var rule = new DB.FilterStringRule
          (
            new DB.ParameterValueProvider(new DB.ElementId(paramId)),
            evaluator,
            subPattern,
            true
          );

          filter = new DB.ElementParameterFilter(rule, inverted);
          pattern = default;
          return true;
        }
      }

      filter = default;
      return false;
    }

    protected static bool TryGetFilterElementIdParam(DB.BuiltInParameter paramId, DB.ElementId pattern, out DB.ElementFilter filter)
    {
      var rule = new DB.FilterElementIdRule
      (
        new DB.ParameterValueProvider(new DB.ElementId(paramId)),
        new DB.FilterNumericEquals(),
        pattern
      );

      filter = new DB.ElementParameterFilter(rule, false);
      return true;
    }
  }
}
