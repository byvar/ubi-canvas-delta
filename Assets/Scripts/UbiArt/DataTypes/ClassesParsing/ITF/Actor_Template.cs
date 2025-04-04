﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace UbiArt.ITF {
	public partial class Actor_Template {
		/// <summary>
		/// Gets first component of type T
		/// </summary>
		/// <typeparam name="T">Component type</typeparam>
		/// <returns>Component of the requested type</returns>
		public T GetComponent<T>() where T : ActorComponent_Template {
			if (COMPONENTS == null) return null;
			return COMPONENTS.FirstOrDefault(c => (c?.obj as T) != null)?.obj as T;
		}
		public IEnumerable<T> GetComponents<T>() where T : ActorComponent_Template {
			if (COMPONENTS == null) return null;
			return COMPONENTS.Where(c => (c?.obj as T) != null).Select(c => c?.obj as T);
		}
		public T AddComponent<T>(T t = null) where T : ActorComponent_Template, new() {
			if (COMPONENTS == null) COMPONENTS = new CArrayO<Generic<ActorComponent_Template>>();
			if (t == null) t = new T();
			COMPONENTS.Add(new Generic<ActorComponent_Template>(t));
			return t;
		}
		public void RemoveComponent<T>(T t = null) where T : ActorComponent_Template, new() {
			if (COMPONENTS == null) COMPONENTS = new CArrayO<Generic<ActorComponent_Template>>();
			if (t == null) t = GetComponent<T>();
			if (t == null) return;
			var cmpObj = COMPONENTS.FirstOrDefault(c => c?.obj == t);
			COMPONENTS.Remove(cmpObj);
		}

		public Actor Instantiate(Path templatePath) {
			var basename = templatePath?.GetFilenameWithoutExtension(removeCooked: true);
			Actor act;
			if (templatePath == new Path("enginedata/actortemplates/subscene.tpl")) {
				act = new SubSceneActor() {
					POS2D = Vec2d.Zero,
					USERFRIENDLY = basename,
					LUA = templatePath,
					template = new GenericFile<Actor_Template>(this),
					templatePickable = this,

					EMBED_SCENE = true,
					ZFORCED = true,
					SCENE = new Nullable<Scene>(new Scene())
				};
				((SubSceneActor)act).SCENE.value.InitContext(UbiArtContext);
			} else {
				act = new Actor() {
					POS2D = Vec2d.Zero,
					USERFRIENDLY = basename,
					LUA = templatePath,
					template = new GenericFile<Actor_Template>(this),
					templatePickable = this,
				};
			}
			act.InitContext(UbiArtContext);
			ActorComponent InstantiateComponent(ActorComponent_Template ctpl) => ctpl?.Instantiate(UbiArtContext);

			act.COMPONENTS = new CArrayO<Generic<ActorComponent>>(COMPONENTS.Select(c => new Generic<ActorComponent>(InstantiateComponent(c.obj))).ToArray());
			var clone = (Actor)act.Clone("act");
			return clone;
		}
	}
}
