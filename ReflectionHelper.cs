using LayerCanopyPhotosynthesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PSAppServerCoreNet.Classes
{
    public class ReflectionHelper
    {
        public static ModelPar getAttributeByName(string modelName, string propertyName)
        {
            PropertyInfo p = GetPropertyByName(modelName, propertyName);
            return GetModelAttribute(p);
        }
        //----------------------------------------------------------------------------------------------------------------
        public static ModelPar GetModelAttribute(PropertyInfo p)
        {
            ModelPar par = null;
            par = (ModelPar)p.GetCustomAttribute(typeof(ModelPar));
            return par;
        }
        //----------------------------------------------------------------------------------------------------------------
        public static object MapInternalModel(string modelName, PhotosynthesisModel ps)
        {
            object model = null;

            if (modelName == "LeafCanopy")
            {
                model = ps.Canopy;
            }
            else if (modelName == "PathwayParameters")
            {
                model = ps.Canopy.CPath;
            }
            else if (modelName == "EnvironmentModel")
            {
                model = ps.EnvModel;
            }
            else if (modelName == "PhotosynthesisModel")
            {
                model = ps;
            }

            return model;
        }
        //----------------------------------------------------------------------------------------------------------------
        public static object GetValueByNameLayer(string modelName, string propertyName, PhotosynthesisModel ps, int layer)
        {
            PropertyInfo p = GetPropertyByName(modelName, propertyName);

            object model = MapInternalModel(modelName, ps);

            double[] values = null;

            values = (double[])p.GetValue(model);

            return values[layer];
        }
        //----------------------------------------------------------------------------------------------------------------
        public static object GetCurveDataByNameLayer(string modelName, string propertyName, PhotosynthesisModel ps, int layer)
        {
            PropertyInfo p = GetPropertyByName(modelName, propertyName);

            return p.GetValue(ps);
        }
        //----------------------------------------------------------------------------------------------------------------
        public static object[] GetLayeredSSValueByNameLayer(string modelName, string propertyName, PhotosynthesisModel ps, int layer)
        {
            PropertyInfo p = GetPropertyByName(modelName, propertyName);

            double[] values = null;
            object[] result = new object[2];

            values = (double[])p.GetValue(ps.Sunlit);
            result[0] = values[0];

            values = (double[])p.GetValue(ps.Shaded);
            result[1] = values[0];

            return result;
        }
        //----------------------------------------------------------------------------------------------------------------
        public static object GetValueByName(string modelName, string propertyName, PhotosynthesisModel ps)
        {
            PropertyInfo p = GetPropertyByName(modelName, propertyName);

            object model = MapInternalModel(modelName, ps);

            return p.GetValue(model);
        }
        //----------------------------------------------------------------------------------------------------------------
        public static void SetValueByName(string modelName, string propertyName, PhotosynthesisModel ps, object value)
        {
            PropertyInfo p = GetPropertyByName(modelName, propertyName);

            object model = MapInternalModel(modelName, ps);

            p.SetValue(model, value);
        }
        //----------------------------------------------------------------------------------------------------------------
        public static PropertyInfo GetPropertyByName(string modelName, string propertyName)
        {
            //Assembly ass = (Assembly)AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name.Contains("LayerCanopyPhotosynthesis"));

            //Type model = ass.GetTypes().First(t => t.Name == modelName);

            Type model = Type.GetType("LayerCanopyPhotosynthesis." + modelName);

            List<PropertyInfo> props = new List<PropertyInfo>(model.GetProperties().Where(p => p.Name == propertyName));

            if (props.Count == 0)
            {
                return null;
            }

            return props[0];
        }
        //----------------------------------------------------------------------------------------------------------------
        public static void GetPropertyByGUID(string guid, out Type model, out PropertyInfo prop)
        {
            model = null;
            prop = null;


            //Assembly ass = (Assembly)AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name.Contains("LayerCanopyPhotosynthesis"));
            //Type ass = Type()
            //List<Type> types = new List<Type>(ass.GetTypes());
            List<Type> types = new List<Type>();

            List<string> modelNames = new List<string>(new string[] { "LeafCanopy", "PathwayParameters", "EnvironmentModel", "PhotosynthesisModel" });


            //Iterate through the types
            foreach (Type t in types)
            {
                //Look for the UID and find the model and the property
                List<PropertyInfo> props = new List<PropertyInfo>(t.GetProperties()
                    .Where(p => p.GetCustomAttribute(typeof(ModelPar)) != null &&
                     ((ModelPar)p.GetCustomAttribute(typeof(ModelPar))).Id == guid));

                if (props.Count > 0)
                {
                    prop = props[0];
                    model = t;
                    return;
                }
            }
        }

        ////----------------------------------------------------------------------------------------------------------------
        //public static PropertyInfo getPropertyByGUID(string guid)
        //{
        //    Assembly ass = (Assembly)AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name.Contains("LayerCanopyPhotosynthesis"));

        //    List<PropertyInfo> allProps = new List<PropertyInfo>(ass.GetType().GetProperties());

        //    List<PropertyInfo> props = new List<PropertyInfo>(allProps
        //        .Where(p => p.GetCustomAttribute(typeof(ModelPar)) != null &&
        //         ((ModelPar)p.GetCustomAttribute(typeof(ModelPar))).id == guid));

        //    if (props.Count == 0)
        //    {
        //        return null;
        //    }

        //    return props[0];
        //}
    }
}
