using System;
using System.Runtime.Serialization;
using ColossalGame.Models;

namespace ColossalGame.Services
{
    public class LoginService : ILoginService
    {
        private readonly UserService _us;

        public LoginService(UserService us)
        {
            _us = us;
        }

        /// <summary>
        /// Using a username and password, this method tries to find an account associated with the username and tests if the password is correct. If so, it will return a token the user can use to verify identity.
        /// </summary>
        /// <param name="username">The username of the user</param>
        /// <param name="password">The password of the user</param>
        /// <returns>The token associated with that user. Be careful, the token only lasts 1 day!</returns>
        public string SignIn(string username, string password)
        {
            if (username==null) throw new BadUsernameException("Username was null");
            if (_us.GetByUsername(username)==null) throw new UserDoesNotExistException();
            var returnedUser = _us.GetByUsername(username);

            if (password!=null&&BCrypt.Net.BCrypt.Verify(password, returnedUser.PasswordHash))
                return _us.generateToken(returnedUser);
            else
                throw new IncorrectPasswordException();
            
            
            
        }

        /// <summary>
        /// Creates an account for the desired username and password
        /// </summary>
        /// <param name="username">The user's desired username</param>
        /// <param name="password">The user's desired password</param>
        /// <returns>A boolean representing whether the account was created successfully or not</returns>
        public bool SignUp(string username, string password)
        {
            if (string.IsNullOrEmpty(username)) throw new BadUsernameException();
            if (string.IsNullOrEmpty(password)) throw new BadPasswordException();
            if (_us.UserExistsByUsername(username)) throw new UserAlreadyExistsException();


            var insertUser = new User();
            insertUser.Username = username;
            
            insertUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, 12);
            _us.Create(insertUser);
            return true;
        }

        /// <summary>
        /// A method that takes in a token and a username and returns whether that token is legitimate for that user. If it is then we can assume that the user is authentic.
        /// </summary>
        /// <param name="token">The user's token</param>
        /// <param name="username">The user's username</param>
        /// <returns>A boolean representing whether the token is legitimate</returns>
        public bool VerifyToken(string token, string username)
        {
            if (!_us.UserExistsByUsername(username)) throw new UserDoesNotExistException();
            var returnedUser = _us.GetByUsername(username);
            var ts = DateTime.Now - returnedUser.TokenAge;
            if (ts.TotalDays >= 1) throw new TokenExpiredException();
            return BCrypt.Net.BCrypt.Verify(token, returnedUser.TokenHash);
        }

        /// <summary>
        /// Deletes specified user
        /// </summary>
        /// <param name="username">User's username</param>
        /// <returns>A boolean representing whether that user was deleted</returns>
        public bool DeleteUser(string username)
        {
            if (!_us.UserExistsByUsername(username))
            {
                return false;
            }
            
            User returnedUser = _us.GetByUsername(username);
            _us.Remove(returnedUser.Id);
            return true;
        }
    }

    [Serializable]
    public class BadUsernameException : Exception
    {
        public BadUsernameException()
        {
        }

        public BadUsernameException(string message) : base(message)
        {
        }

        public BadUsernameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BadUsernameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class BadPasswordException : Exception
    {
        public BadPasswordException()
        {
        }

        public BadPasswordException(string message) : base(message)
        {
        }

        public BadPasswordException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BadPasswordException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class UserDoesNotExistException : Exception
    {
        public UserDoesNotExistException()
        {
        }

        public UserDoesNotExistException(string message) : base(message)
        {
        }

        public UserDoesNotExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserDoesNotExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException()
        {
        }

        public UserAlreadyExistsException(string message) : base(message)
        {
        }

        public UserAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class IncorrectPasswordException : Exception
    {
        public IncorrectPasswordException()
        {
        }

        public IncorrectPasswordException(string message) : base(message)
        {
        }

        public IncorrectPasswordException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IncorrectPasswordException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class TokenExpiredException : Exception
    {
        public TokenExpiredException()
        {
        }

        public TokenExpiredException(string message) : base(message)
        {
        }

        public TokenExpiredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TokenExpiredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public interface ILoginService
    {
        public bool SignUp(string username, string password);
        public string SignIn(string username, string password);
        public bool VerifyToken(string token, string username);
        public bool DeleteUser(string username);
    }
}