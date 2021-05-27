using PostSharp.Extensibility;

using System;
using System.Linq;
using System.Reflection;
/**
 * @author    cfHxqA
 * @copyright 2013 - 2021 (C) by cfHxqA
 *
 * @package   PostSharp
 * @category  PostSharp.Constraints
 *
 * @license   Attribution-NonCommercial-NoDerivs 4.0 Unported <http://creativecommons.org/licenses/by-nc-nd/4.0/>
 */
namespace PostSharp.Constraints
{
  [MulticastAttributeUsage(MulticastTargets.Class, Inheritance = MulticastInheritance.None)]
  public sealed class OnSingletonContstraint : ScalarConstraint {
    /// <summary>
    /// Validates the element of code to which the constraint is applied.
    /// </summary>
    /// <remarks>
    ///   The singleton pattern is as follows:
    ///   public static $CLASS$ GetInstance() => Instance.Value;
    ///   private static readonly Lazy<Test> Instance = new Lazy<$CLASS$>(() => new $CLASS$(), true);
    ///   private $CLASS$() { }
    /// </remarks>
    /// <param name="target">Element of code to which the constraint is applied (<see cref="T:System.Reflection.Assembly" />, <see cref="T:System.Type" />,
    /// <see cref="T:System.Reflection.MethodInfo" />, <see cref="T:System.Reflection.ConstructorInfo" />, <see cref="T:System.Reflection.PropertyInfo" />,
    /// <see cref="T:System.Reflection.EventInfo" />, <see cref="T:System.Reflection.FieldInfo" />, <see cref="T:System.Reflection.ParameterInfo" />).</param>
    public override void ValidateCode(object target) {
      Type TargetType = (Type)target;

      Type GenericType = typeof(Lazy<>).MakeGenericType(TargetType);
      FieldInfo LazyField = TargetType.GetField("Instance", BindingFlags.NonPublic | BindingFlags.Static);


      if (LazyField is null || !LazyField.IsPrivate || LazyField.FieldType != GenericType || !LazyField.IsInitOnly)
        Message.Write(TargetType, SeverityType.Error, "2001", "The {0} type does not have 'private static readonly Lazy<{0}> Instance = new Lazy<{0}>(() => new {0}(), true);'.", TargetType.Name);

      MethodInfo Property = TargetType.GetMethod("GetInstance", BindingFlags.Public | BindingFlags.Static);

      if (Property is null || Property.ReturnType != TargetType)
        Message.Write(TargetType, SeverityType.Error, "2002", "The {0} type does not have 'public static {0} GetInstance() => Instance.Value;'.", TargetType.Name);
      else {
        if (!TargetType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Any(Constructor => Constructor.IsPublic && (Constructor.GetParameters().Length == 0)))
          Message.Write(TargetType, SeverityType.Error, "2003", "The {0} type does not have a single, parameterless public constructor.", TargetType.Name);
      } // end statement
    }
  }
}
 