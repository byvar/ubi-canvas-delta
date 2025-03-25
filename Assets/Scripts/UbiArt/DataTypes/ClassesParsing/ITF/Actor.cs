using System;
using System.Collections.Generic;
using System.Linq;

namespace UbiArt.ITF {
	public partial class Actor {
		public GenericFile<Actor_Template> template;

		protected override void OnPostSerialize(CSerializerObject s) {
			base.OnPostSerialize(s);
			if (this is Frise) return;
			if (IsFirstLoad) {
				Loader l = s.Context.Loader;
				l.AddLoadedActor(this);
				l.LoadFile<GenericFile<Actor_Template>>(LUA, result => {
					template = result;
					templatePickable = template?.obj;
				});
			}
		}

		protected override void OnPreSerialize(CSerializerObject s) {
			base.OnPreSerialize(s);
		}

		/// <summary>
		/// Gets first component of type T
		/// </summary>
		/// <typeparam name="T">Component type</typeparam>
		/// <returns>Component of the requested type</returns>
		public T GetComponent<T>() where T : ActorComponent {
			if (COMPONENTS == null) return null;
			return COMPONENTS.FirstOrDefault(c => (c?.obj as T) != null)?.obj as T;
		}
		public IEnumerable<T> GetComponents<T>() where T : ActorComponent {
			if (COMPONENTS == null) return null;
			return COMPONENTS.Where(c => (c?.obj as T) != null).Select(c => c?.obj as T);
		}

		public T AddComponent<T>(T t = null) where T : ActorComponent, new() {
			if (COMPONENTS == null) COMPONENTS = new CArrayO<Generic<ActorComponent>>();
			if (t == null) t = new T();
			COMPONENTS.Add(new Generic<ActorComponent>(t));
			return t;
		}
		public void RemoveComponent<T>(T t = null) where T : ActorComponent, new() {
			if (COMPONENTS == null) COMPONENTS = new CArrayO<Generic<ActorComponent>>();
			if (t == null) t = GetComponent<T>();
			if (t == null) return;
			var cmpObj = COMPONENTS.FirstOrDefault(c => c?.obj == t);
			COMPONENTS.Remove(cmpObj);
		}
	}
}
