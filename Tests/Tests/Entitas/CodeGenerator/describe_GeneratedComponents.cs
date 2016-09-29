using NSpec;
using Entitas;
using System;

class describe_GeneratedComponents : nspec {

    void when_generated() {

        Pool pool = null;

        before = () => {
            pool = new Pool(ComponentIds.TotalComponents);
        };

        context["component without fields"] = () => {

            Entity e = null;
            int index = -1;

            before = () => {
                e = pool.CreateEntity();
                e.isMovable = true;
                index = ComponentIds.Movable;
            };

            it["adds component"] = () => {
                e.HasComponent(index).should_be_true();
                e.isMovable.should_be_true();
            };

            it["removes component"] = () => {
                e.isMovable = false;

                e.HasComponent(index).should_be_false();
                e.isMovable.should_be_false();
            };

            it["adds same instance"] = () => {
                var component = e.GetComponent(index);

                e.isMovable = false;
                e.isMovable = true;

                e.GetComponent(index).should_be_same(component);
            };

            it["adds same instance to multiple entities"] = () => {
                var e2 = pool.CreateEntity();
                e2.isMovable = true;

                e.GetComponent(index).should_be_same(e2.GetComponent(index));
            };

            it["ignores setting to true multiple times"] = () => {
                e.isMovable = true;
                e.isMovable = true;
            };

            it["ignores setting to false multiple times"] = () => {
                e.isMovable = false;
                e.isMovable = false;
            };

            context["matcher"] = () => {

                IMatcher matcher = null;

                before = () => {
                    matcher = Matcher.Movable;
                };

                it["generates matcher"] = () => {
                    matcher.indices.Length.should_be(1);
                    matcher.indices[0].should_be(index);
                };

                it["gets same instance"] = () => {
                    matcher.should_be_same(Matcher.Movable);
                };

                it["has component names"] = () => {
                    ((Matcher)matcher).componentNames.should_be(ComponentIds.componentNames);
                };
            };
        };

        context["component with fields"] = () => {

            Entity e = null;
            int index = ComponentIds.Person;

            before = () => {
                e = pool.CreateEntity();
            };

            it["adds component"] = () => {
                e.AddPerson(42, "Max");
                e.HasComponent(index).should_be_true();
                e.hasPerson.should_be_true();

                e.person.age.should_be(42);
                e.person.name.should_be("Max");
            };

            it["removes component"] = () => {
                e.AddPerson(42, "Max");
                e.RemovePerson();

                e.HasComponent(index).should_be_false();
                e.hasPerson.should_be_false();
            };

            it["replaces component"] = () => {
                e.AddPerson(42, "Max");
                e.ReplacePerson(24, "John");

                e.person.age.should_be(24);
                e.person.name.should_be("John");
            };

            it["adds component when using replace component doesn't exist"] = () => {
                e.ReplacePerson(24, "John");

                e.person.age.should_be(24);
                e.person.name.should_be("John");
            };

            context["component pool"] = () => {
                
                PersonComponent person = null;

                before = () => {
                    person = e.AddPerson(42, "Max").person;
                };

                it["reuses component when remove"] = () => {
                    e.RemovePerson();
                    e.AddPerson(24, "John");

                    e.person.should_be_same(person);
                    e.person.age.should_be(24);
                    e.person.name.should_be("John");
                };

                it["reuses component when replace"] = () => {
                    e.ReplacePerson(24, "John");

                    e.person.should_not_be_same(person);

                    e.ReplacePerson(12, "Jack");

                    e.person.should_be_same(person);
                    e.person.age.should_be(12);
                    e.person.name.should_be("Jack");
                };

                it["reuses component when destroying entity"] = () => {
                    pool.DestroyEntity(e);

                    e = pool.CreateEntity();
                    e.AddPerson(24, "John");

                    e.person.should_be_same(person);
                    e.person.age.should_be(24);
                    e.person.name.should_be("John");
                };
            };

            context["matcher"] = () => {

                IMatcher matcher = null;

                before = () => {
                    matcher = Matcher.Person;
                };

                it["generates matcher"] = () => {
                    matcher.indices.Length.should_be(1);
                    matcher.indices[0].should_be(index);
                };

                it["gets same instance"] = () => {
                    matcher.should_be_same(Matcher.Person);
                };

                it["has component names"] = () => {
                    ((Matcher)matcher).componentNames.should_be(ComponentIds.componentNames);
                };
            };
        };

        context["single component without fields"] = () => {

            Entity e = null;
            int index = -1;

            before = () => {
                index = ComponentIds.Animating;
            };

            context["entity extensions"] = () => {

                before = () => {
                    e = pool.CreateEntity();
                    e.isAnimating = true;
                };

                it["adds component"] = () => {
                    e.HasComponent(index).should_be_true();
                    e.isAnimating.should_be_true();
                };

                it["removes component"] = () => {
                    e.isAnimating = false;

                    e.HasComponent(index).should_be_false();
                    e.isAnimating.should_be_false();
                };

                it["adds same instance"] = () => {
                    var component = e.GetComponent(index);

                    e.isAnimating = false;
                    e.isAnimating = true;

                    e.GetComponent(index).should_be_same(component);
                };

                it["adds same instance to multiple entities"] = () => {
                    var e2 = pool.CreateEntity();
                    e2.isAnimating = true;

                    e.GetComponent(index).should_be_same(e2.GetComponent(index));
                };

                it["ignores setting to true multiple times"] = () => {
                    e.isAnimating = true;
                    e.isAnimating = true;
                };

                it["ignores setting to false multiple times"] = () => {
                    e.isAnimating = false;
                    e.isAnimating = false;
                };

                context["matcher"] = () => {

                    IMatcher matcher = null;

                    before = () => {
                        matcher = Matcher.Animating;
                    };

                    it["generates matcher"] = () => {
                        matcher.indices.Length.should_be(1);
                        matcher.indices[0].should_be(index);
                    };

                    it["gets same instance"] = () => {
                        matcher.should_be_same(Matcher.Animating);
                    };

                    it["has component names"] = () => {
                        ((Matcher)matcher).componentNames.should_be(ComponentIds.componentNames);
                    };
                };
            };

            context["pool extensions"] = () => {

                before = () => {
                    pool.isAnimating = true;
                };

                it["creates entity"] = () => {
                    var singleEntity = pool.GetGroup(Matcher.Animating).GetSingleEntity();
                    singleEntity.should_not_be_null();

                    pool.animatingEntity.should_be_same(singleEntity);

                    pool.isAnimating.should_be_true();
                };

                it["destroys entity"] = () => {
                    pool.isAnimating = false;

                    var singleEntity = pool.GetGroup(Matcher.Animating).GetSingleEntity();
                    singleEntity.should_be_null();

                    pool.animatingEntity.should_be_null();

                    pool.isAnimating.should_be_false();

                    pool.count.should_be(0);
                };

                it["doesn't create entity if it already exists"] = () => {
                    var animatingEntity = pool.animatingEntity;
                    pool.isAnimating = true;
                    pool.animatingEntity.should_be_same(animatingEntity);
                    pool.isAnimating = true;
                    pool.animatingEntity.should_be_same(animatingEntity);
                    pool.GetEntities(Matcher.Animating).Length.should_be(1);
                };

                it["ignores setting to false multiple times"] = () => {
                    pool.isAnimating = false;
                    pool.isAnimating = false;
                    pool.animatingEntity.should_be_null();
                };

                it["destroys entity even if it has other components"] = () => {
                    pool.animatingEntity.AddPerson(42, "Max");

                    pool.isAnimating = false;

                    var singleEntity = pool.GetGroup(Matcher.Animating).GetSingleEntity();
                    singleEntity.should_be_null();
                    pool.animatingEntity.should_be_null();

                    pool.count.should_be(0);
                };
            };
        };

        context["single component with fields"] = () => {

            int index = ComponentIds.User;
            var date1 = new DateTime(1);
            var date2 = new DateTime(2);

            context["entity extensions"] = () => {

                Entity e = null;

                before = () => {
                    e = pool.CreateEntity();
                };

                it["adds component"] = () => {
                    e.AddUser(date1, false);
                    e.HasComponent(index).should_be_true();
                    e.hasUser.should_be_true();

                    e.user.timestamp.should_be(date1);
                    e.user.isLoggedIn.should_be_false();
                };

                it["removes component"] = () => {
                    e.AddUser(date1, false);
                    e.RemoveUser();

                    e.HasComponent(index).should_be_false();
                    e.hasUser.should_be_false();
                };

                it["replaces component"] = () => {
                    e.AddUser(date1, false);
                    e.ReplaceUser(date2, true);

                    e.user.timestamp.should_be(date2);
                    e.user.isLoggedIn.should_be_true();
                };

                it["adds component when using replace component doesn't exist"] = () => {
                    e.ReplaceUser(date2, true);

                    e.user.timestamp.should_be(date2);
                    e.user.isLoggedIn.should_be_true();
                };

                context["component pool"] = () => {

                    UserComponent user = null;

                    before = () => {
                        user = e.AddUser(date1, false).user;
                    };

                    it["reuses component when remove"] = () => {
                        e.RemoveUser();
                        e.AddUser(date2, true);

                        e.user.should_be_same(user);
                        e.user.timestamp.should_be(date2);
                        e.user.isLoggedIn.should_be_true();
                    };

                    it["reuses component when replace"] = () => {
                        e.ReplaceUser(date2, true);

                        e.user.should_not_be_same(user);

                        e.ReplaceUser(date1, false);

                        e.user.should_be_same(user);
                        e.user.timestamp.should_be(date1);
                        e.user.isLoggedIn.should_be_false();
                    };

                    it["reuses component when destroying entity"] = () => {
                        pool.DestroyEntity(e);

                        e = pool.CreateEntity();
                        e.AddUser(date2, true);

                        e.user.should_be_same(user);
                        e.user.timestamp.should_be(date2);
                        e.user.isLoggedIn.should_be_true();
                    };
                };

                context["matcher"] = () => {

                    IMatcher matcher = null;

                    before = () => {
                        matcher = Matcher.User;
                    };

                    it["generates matcher"] = () => {
                        matcher.indices.Length.should_be(1);
                        matcher.indices[0].should_be(index);
                    };

                    it["gets same instance"] = () => {
                        matcher.should_be_same(Matcher.User);
                    };

                    it["has component names"] = () => {
                        ((Matcher)matcher).componentNames.should_be(ComponentIds.componentNames);
                    };
                };
            };

            context["pool extensions"] = () => {

                it["creates entity"] = () => {
                    var userEntity = pool.SetUser(date1, false);

                    var singleEntity = pool.GetGroup(Matcher.User).GetSingleEntity();
                    singleEntity.should_be_same(userEntity);

                    pool.userEntity.should_be_same(userEntity);
                    pool.hasUser.should_be_true();
                    pool.user.should_be_same(userEntity.user);

                    userEntity.user.timestamp.should_be(date1);
                    userEntity.user.isLoggedIn.should_be_false();
                };

                it["throws when creating the entity twice"] = expect<EntitasException>(() => {
                    pool.SetUser(date1, false);
                    pool.SetUser(date2, true);
                });

                it["replaces component on existing entity"] = () => {
                    pool.SetUser(date1, false);
                    var userEntity = pool.userEntity;
                    pool.ReplaceUser(date2, true);

                    pool.userEntity.should_be_same(userEntity);
                    pool.userEntity.user.timestamp.should_be(date2);
                    pool.userEntity.user.isLoggedIn.should_be_true();
                };

                it["creates entity when using replace but entity doesn't exist"] = () => {
                    pool.ReplaceUser(date2, true);

                    pool.userEntity.user.timestamp.should_be(date2);
                    pool.userEntity.user.isLoggedIn.should_be_true();

                    pool.GetEntities(Matcher.User).Length.should_be(1);
                };

                it["destroys entity"] = () => {
                    pool.SetUser(date1, false);
                    pool.RemoveUser();

                    var singleEntity = pool.GetGroup(Matcher.User).GetSingleEntity();
                    singleEntity.should_be_null();

                    pool.userEntity.should_be_null();
                    pool.hasUser.should_be_false();

                    pool.count.should_be(0);
                };

                it["destroys entity even if it has other components"] = () => {
                    var userEntity = pool.SetUser(date1, false);
                    userEntity.isMovable = true;
                    pool.RemoveUser();

                    var singleEntity = pool.GetGroup(Matcher.User).GetSingleEntity();
                    singleEntity.should_be_null();

                    pool.userEntity.should_be_null();
                    pool.hasUser.should_be_false();

                    pool.count.should_be(0);
                };

                it["throws when trying to get component but entity doesn't exist"] = expect<NullReferenceException>(() => {
                    var user = pool.user;
                });
            };
        };
    }
}
