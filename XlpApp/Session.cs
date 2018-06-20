using System;
using System.Collections.Generic;
using System.Text;
using XlpApp.Properties;
using System.ComponentModel;
using JEXEServerLib;

namespace XlpApp
{
    /// <summary>
    /// Created by: Alexander M. Rufon
    /// Year: 2006
    /// EMail: amrufon@gmail.com
    /// </summary>
    /// <remarks>A class to use J in .NET Framework</remarks>
    public class Session : IDisposable
    {
        /// <summary>
        /// Holds the instance of the J Object
        /// </summary>
        private JEXEServerClass jObject;
        /// <summary>
        /// Holds the debugging flag value
        /// </summary>
        private bool debug = false;
        /// <summary>
        /// Holds the flag for the IDisposed state
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Create an J session with the debugging state specified
        /// </summary>
        /// <param name="debug">Set to true if you want debugging turned on</param>
        public Session(bool debug)
        {
            this.debug = debug;
            this.initialize();
        }

        /// <summary>
        /// Creates an instance of J with default settings
        /// </summary>
        public Session() { this.initialize(); }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Session()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        /// <summary>
        /// Read-Only property which returns true if object was created with debugging code turned on.
        /// </summary>
        public bool debugging { get { return this.debug; } }

        /// <summary>
        /// Actual method that initializes the session
        /// </summary>
        private void initialize()
        {
            string jScript = "";

            // Create a new copy of the J Object and make sure were in the Z locale
            jObject = new JEXEServerClass();
            jObject.Quit();
            jObject.Log(1);
            jObject.Show(1);
            this.Eval("18!:4 <'z'");

            //// Now get the base J Script
            //jScript = UnicodeEncoding.ASCII.GetString(Resources.script);
            //// If were debugging, show the J Session Window
            //if (this.debug)
            //{
            //    // Display the session and load the profile
            //    jScript += "\nshowJ ''";
            //    jScript += "\nloadprofile ''";
            //}
            //// load this script to the current session
            //this.Variable("baseScript", jScript);

            //// We execute the script using the code stored in the resource file
            //jScript = Resources.ScriptLoader;
            //// Load the new script to the current session
            //this.Variable("loadScript", jScript);

            //// Now we execute the values in the loadScript variable
            //this.Eval("0!:100 loadScript");

            if (this.debug)
            {
                // Just display a fun message
                string script = "NB. Have fun! - bathala <amrufon@gmail.com>";
                this.Eval(script);
            }
        }

        /// <summary>
        /// Returns the contents of an J variable
        /// </summary>
        /// <param name="name">Variable name to retrieve</param>
        public object Variable(string name)
        {
            object retVal;

            // Retrieve the data from the J Session
            jObject.GetB(name, out retVal);

            // Return the object
            return retVal;
        }

        /// <summary>
        /// Sets an J variable to a value
        /// </summary>
        /// <param name="name">Variable name to set</param>
        /// <param name="value">The value to set the J variable to</param>
        public void Variable(string name, object value) { jObject.SetB(name, ref value); }

        /// <summary>
        /// Sets a J variable to a string value
        /// </summary>
        /// <param name="name">Variable name to set</param>
        /// <param name="value">String value to fill the variable name</param>
        public void Variable(string name, string value)
        {
            object objTemp = value;
            this.Variable(name, objTemp);
        }

        /// <summary>
        /// Set a variable to an int value
        /// </summary>
        public void Variable(string name, int value)
        {
            object objTemp = value;
            this.Variable(name, objTemp);
        }

        /// <summary>
        /// Set a variable to a DateTime value
        /// </summary>
        public void Variable(string name, DateTime value)
        {
            string objTemp = value.ToString("MM/dd/yyyy HH:mm:ss");
            this.Variable(name, objTemp);
        }

        /// <summary>
        /// Set a variable to a float value
        /// </summary>
        public void Variable(string name, double value)
        {
            object objTemp = value;
            this.Variable(name, objTemp);
        }

        /// <summary>
        /// Set a variable to a bool value
        /// </summary>
        public void Variable(string name, bool value)
        {
            object objTemp;
            if (value)
            {
                objTemp = 1;
            }
            else
            {
                objTemp = 0;
            }
            this.Variable(name, objTemp);
        }

        /// <summary>
        /// Evaluates J scripts
        /// </summary>
        /// <param name="command">J script to evaluate</param>
        public void Eval(string command)
        {
            try
            {
                int result;

                // Execute the command
                result = jObject.Do(command);

                if (result > 0)
                {
                    // Throw the correct error message
                    object errorMessage;
                    jObject.ErrorTextB(result, out errorMessage);
                    Exception eoe = new Exception(Convert.ToString(errorMessage));
                    throw eoe;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Loads an external script into the current J session
        /// </summary>
        /// <param name="fileName">Complete path and filename to the script to be loaded</param>
        public void Load(string fileName)
        {
            string script;

            // Assign the filename to a J variable
            this.Variable("script2load", fileName);

            // Check if were in debug mode first.
            if (this.debug)
            {
                // Were debugging so we show what were loading and stop on error
                script = "0!:001 < script2load";
            }
            else
            {
                // Not debugging, dont need to show script
                script = "0!:0 < script2load";
            }

            // Now evaluate the script.
            this.Eval(script);
        }

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virutal.
        /// A derived class should not be able to overrride this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing the second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    // Component.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed
                // We basically call it quits and set the JObject to nothing
                if (jObject != null)
                {
                    jObject.Quit();
                    jObject = null;
                }

                // Force garbage collection
                GC.Collect();
            }
            disposed = true;
        }
    }
}
