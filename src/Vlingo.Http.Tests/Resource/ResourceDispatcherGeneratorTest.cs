// Copyright © 2012-2018 Vaughn Vernon. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using Vlingo.Http.Resource;
using Vlingo.Http.Tests.Sample.User;
using Xunit;
using Xunit.Abstractions;
using Action = Vlingo.Http.Resource.Action;

namespace Vlingo.Http.Tests.Resource
{
    public class ResourceDispatcherGeneratorTest
    {
        private readonly List<Action> _actions;
        private ConfigurationResource<UserResource> _resource;

        [Fact]
        public void TestSourceCodeGeneration()
        {
            var generator = ResourceDispatcherGenerator.ForTest(_actions, false);

            var result = generator.GenerateFor(_resource.ResourceHandlerClass);

            Assert.NotNull(result);
            Assert.NotNull(result.SourceFile);
            Assert.False(result.SourceFile.Exists);
            Assert.NotNull(result.FullyQualifiedClassName);
            Assert.NotNull(result.ClassName);
            Assert.NotNull(result.Source);
        }
        
        [Fact]
        public void TestSourceCodeGenerationWithPersistence()
        {
            var generator = ResourceDispatcherGenerator.ForTest(_actions, true);

            var result = generator.GenerateFor(_resource.ResourceHandlerClass);

            Assert.NotNull(result);
            Assert.NotNull(result.SourceFile);
            Assert.True(result.SourceFile.Exists);
            Assert.NotNull(result.FullyQualifiedClassName);
            Assert.NotNull(result.ClassName);
            Assert.NotNull(result.Source);
        }
    
        public ResourceDispatcherGeneratorTest(ITestOutputHelper output)
        {
            var converter = new Converter(output);
            Console.SetOut(converter);
            
            var actionPostUser = new Action(0, "POST", "/users", "register(body:Vlingo.Http.Tests.Sample.User.UserData userData)", null, true);
            var actionPatchUserContact = new Action(1, "PATCH", "/users/{userId}/contact", "changeContact(string userId, body:Vlingo.Http.Tests.Sample.User.ContactData contactData)", null, true);
            var actionPatchUserName = new Action(2, "PATCH", "/users/{userId}/name", "changeName(string userId, body:Vlingo.Http.Tests.Sample.User.NameData nameData)", null, true);
            var actionGetUser = new Action(3, "GET", "/users/{userId}", "queryUser(string userId)", null, true);
            var actionGetUsers = new Action(4, "GET", "/users", "queryUsers()", null, true);
            var actionQueryUserError = new Action(5, "GET", "/users/{userId}/error", "queryUserError(string userId)", null, true);
            
            _actions = new List<Action>
            {
                actionPostUser,
                actionPatchUserContact,
                actionPatchUserName,
                actionGetUser,
                actionGetUsers,
                actionQueryUserError
            };

            Type resourceHandlerClass;
            
            try
            {
                resourceHandlerClass = Type.GetType("Vlingo.Http.Tests.Sample.User.UserResource");
            }
            catch (Exception)
            {
                resourceHandlerClass = ConfigurationResource<UserResource>.NewResourceHandlerTypeFor("Vlingo.Http.Tests.Sample.User.UserResource");
            }
            
            _resource = ConfigurationResource<UserResource>.NewResourceFor("user", resourceHandlerClass, 5, _actions);
        }
    }
}