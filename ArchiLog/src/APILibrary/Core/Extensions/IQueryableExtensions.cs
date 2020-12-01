using APILibrary.core.Attributes;
using APILibrary.core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace APILibrary.Core.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<TModel> OrderByDynamic<TModel>(this IQueryable<TModel> query, string asc, string desc) where TModel : ModelBase
        {
            var collectionType = typeof(TModel);
            ParameterExpression parametres = Expression.Parameter(collectionType, "x");

            IOrderedQueryable<TModel> orderedQuery = null;

            int indexAll = 0;
            if (!string.IsNullOrEmpty(asc))
            {
                int indexAsc = 0;
                var tabAsc = asc.Split(',');
                foreach (var itemAsc in tabAsc)
                {
                    var propInModel = collectionType.GetProperty(itemAsc, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                    if (propInModel != null)
                    {
                        var property = Expression.Property(parametres, itemAsc);
                        var body = Expression.Convert(property, typeof(object));
                        var lambda = Expression.Lambda<Func<TModel, object>>(body, parametres);
                        orderedQuery = (indexAsc == 0 && indexAll == 0) ? query.OrderBy(lambda) : orderedQuery.ThenBy(lambda);
                    }
                    indexAll++;
                    indexAsc++;
                }
            }
            if (!string.IsNullOrEmpty(desc))
            {
                int indexDesc = 0;
                var tabDesc = desc.Split(',');
                foreach (var itemDesc in tabDesc)
                {
                    var propInModel = collectionType.GetProperty(itemDesc, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                    if (propInModel != null)
                    {
                        var property = Expression.Property(parametres, itemDesc);
                        var body = Expression.Convert(property, typeof(object));
                        var lambda = Expression.Lambda<Func<TModel, object>>(body, parametres);
                        orderedQuery = (indexDesc == 0 && indexAll == 0) ? query.OrderByDescending(lambda) : orderedQuery.ThenByDescending(lambda);
                    }
                    indexAll++;
                    indexDesc++;
                }
            }
            return orderedQuery == null ? query : orderedQuery;
        }
        public static IQueryable<T> TakePageResult<T>(this IQueryable<T> query, int pageFrom, int pageTo)
        {
            /*   if (query == null)
               {
                   throw new ArgumentNullException(nameof(query));
               }

               if (pageData == null)
               {
                   throw new ArgumentNullException(nameof(pageData));
               }

               if (pageData.PageNr <= 0)
               {
                   throw new ArgumentException(nameof(IPageQuery.PageNr) + " should be equal and greater than 1.");
               }

               if (pageData.PageSize <= 0)
               {
                   throw new ArgumentException("Invalid page size.");
               }
            */
            return query.Skip(pageFrom + 1).Take(pageTo + 1);
        }

        public static IQueryable<TModel> WhereDynamic<TModel>(this IQueryable<TModel> query, IQueryCollection requestQuery, bool isSearch = false) where TModel : ModelBase
        {
            Expression<Func<TModel, bool>> allExpresions = null;

            // prendre le type de TModel
            var collectionType = typeof(TModel);
            ParameterExpression parametres = Expression.Parameter(collectionType, "x");

            // boucler les paramètres qu'on a reçu de l'url
            foreach (var prop in requestQuery)
            {
                // voir si le paramètre existe dans le TModel
                var propInTModel = collectionType.GetProperty(prop.Key, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                if (propInTModel != null)
                {
                    List<string> values = new List<string>();
                    var propString = prop.Value.ToString();

                    // check si la valeur du paramètre contient une virgule (signe de OU)
                    var containsComma = propString.Contains(",");
                    // check si la valeur du paramètre est entre 2 crochets
                    var containsBrackets = propString.StartsWith("[") && propString.EndsWith("]");
                    var containsAsterisks = propString.StartsWith("*") || propString.EndsWith("*");

                    // si contient virgule, faire un split et ajouter les valeurs dans une liste de valeurs
                    if (containsComma)
                        values = propString.Split(",").ToList<string>();
                    // si ne contient pas, ajouter la valeur dans une liste de valeurs (en élément 0)
                    else
                        values.Add(propString);
                    // valeur pour comparaison final des valeurs (OU / ET)
                    Expression<Func<TModel, bool>> expression = null;
                    // boucle des valeurs dans la Liste de valeurs
                    foreach (var val in values)
                    {
                        // get la valeur de VAL sans crochets
                        var withoutBrackets = val.StartsWith("[") ? val.Substring(1) : val;
                        withoutBrackets = withoutBrackets.EndsWith("]") ? withoutBrackets.Substring(0, withoutBrackets.Length - 1) : withoutBrackets;

                        var withoutAsterisks = val.StartsWith("*") ? val.Substring(1) : val;
                        withoutAsterisks = withoutAsterisks.EndsWith("*") ? withoutAsterisks.Substring(0, withoutAsterisks.Length - 1) : withoutAsterisks;
                        // si la valeur n'est pas vide
                        if (!string.IsNullOrEmpty(withoutBrackets) && !string.IsNullOrEmpty(withoutAsterisks))
                        {
                            // convertir en type de l'objet dans TModel
                            var objet = TypeDescriptor.GetConverter(propInTModel.PropertyType).ConvertFromString(withoutBrackets);
                            // comparateur de gauche (la valeur dans le model)
                            Expression valueFromCol = Expression.PropertyOrField(parametres, prop.Key);
                            // comparateur de droite (la valeur saisie par l'utilisateur)
                            Expression valueToCompare = Expression.Constant(objet);
                            // faire la comparaison // dans le cas où pas de crochets
                            Expression makeComparaison = Expression.Equal(valueFromCol, valueToCompare);
                            // s'il y a des crochets
                            if (containsBrackets && !string.IsNullOrEmpty(withoutBrackets))
                            {
                                // si la valeur initiale commence par [ // supérieur ou égal
                                if (val.StartsWith("["))
                                    makeComparaison = Expression.GreaterThanOrEqual(valueFromCol, valueToCompare);
                                // si la valeur initiale finit par [ // inférieur ou égal
                                if (val.EndsWith("]"))
                                    makeComparaison = Expression.LessThanOrEqual(valueFromCol, valueToCompare);
                            }

                            if (containsAsterisks && isSearch)
                            {
                                makeComparaison = Expression.Equal(
                                    Expression.Call(valueFromCol, typeof(String).GetMethods()
                                    .Where(x => x.Name == "Contains")
                                    .FirstOrDefault(x => x.GetParameters().Length == 1), new Expression[] { Expression.Constant(withoutAsterisks) }),
                                    Expression.Constant(true));
                            }

                            var sameExpression = Expression.Lambda<Func<TModel, bool>>(makeComparaison, parametres);
                            // si la valeur de la comparaison final de VALUES est vide
                            if (expression == null)
                            {
                                // assigner le result de la première expression
                                expression = sameExpression;
                            }
                            // sinon
                            else
                            {
                                // comparer l'expression finale à l'expression en cours
                                BinaryExpression binSame = containsBrackets ? Expression.And(expression.Body, sameExpression.Body) : Expression.Or(expression.Body, sameExpression.Body);
                                // puis assigner l'expression reçue à l'expression finale de VALUES
                                expression = Expression.Lambda<Func<TModel, bool>>(binSame, parametres);
                            }
                        }
                    }


                    if (allExpresions == null)
                    {
                        allExpresions = expression;
                    }
                    else
                    {
                        BinaryExpression bin = Expression.And(allExpresions.Body, expression.Body);
                        allExpresions = Expression.Lambda<Func<TModel, bool>>(bin, parametres);
                    }
                }
            }
            return allExpresions == null ? query : query.Where(allExpresions);
        }

        public static object SelectObject(object value, string[] fields)
        {
            var expo = new ExpandoObject() as IDictionary<string, object>;
            var collectionType = value.GetType();

            foreach (var field in fields)
            {
                var prop = collectionType.GetProperty(field, BindingFlags.Public |
                    BindingFlags.IgnoreCase | BindingFlags.Instance);
                if (prop != null)
                {
                    var isPresentAttribute = prop.CustomAttributes
                         .Any(x => x.AttributeType == typeof(NotJsonAttribute));
                    if (!isPresentAttribute)
                        expo.Add(prop.Name, prop.GetValue(value));
                }
                else
                {
                    throw new Exception($"Property {field} does not exist.");
                }
            }
            return expo;
        }

        /*public static IQueryable<dynamic> SelectDynamic<TModel>(this IQueryable<TModel> query, string[] fields) where TModel : ModelBase
           {
               var parameter = Expression.Parameter(typeof(TModel), "x");
               var membersExpression = fields.Select(y => Expression.Property(parameter, y));
               var membersAssignment = membersExpression.Select(z => Expression.Bind(z.Member, z));
               var body = Expression.MemberInit(Expression.New(typeof(TModel)), membersAssignment);
               var lambda = Expression.Lambda<Func<TModel, dynamic>>(body, parameter);
               return query.Select(lambda);
           }*/

        public static IQueryable<TModel> SelectModel<TModel>(this IQueryable<TModel> query, string[] fields) where TModel : ModelBase
        {
            var parameter = Expression.Parameter(typeof(TModel), "x");
            var membersExpression = fields.Select(y => Expression.Property(parameter, y));
            var membersAssignment = membersExpression.Select(z => Expression.Bind(z.Member, z));
            var body = Expression.MemberInit(Expression.New(typeof(TModel)), membersAssignment);
            var lambda = Expression.Lambda<Func<TModel, TModel>>(body, parameter);
            return query.Select(lambda);
        }
    }

}