using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.Search;

namespace UbiArt.ITF {
	public partial class Scene {
		// Returns whether actor was added to scene successfully
		public bool AddActor(Actor a) => AddActor(a, a?.USERFRIENDLY);
		public bool AddActor(Actor a, string name) {
			if(a == null) return false;

			if (UbiArtContext?.Settings?.EngineVersion == EngineVersion.RO) {
				if(ACTORS_ORIGINS == null) ACTORS_ORIGINS = new CArrayO<Generic<Pickable>>();
				if(FRISE_ORIGINS == null) FRISE_ORIGINS = new CArrayO<Generic<Pickable>>();
				if (a != null && !ACTORS_ORIGINS.Any(ac => ac?.obj == a) && !FRISE_ORIGINS.Any(f => f?.obj == a)) {
					if (string.IsNullOrEmpty(name))
						name = a.USERFRIENDLY;
					if (string.IsNullOrEmpty(name))
						name = a.LUA?.GetFilenameWithoutExtension(removeCooked: true);

					if (name != null) {
						var newName = name;
						newName = GetNewActorName(newName);
						a.USERFRIENDLY = newName;
					}
					if (a is Frise f) {
						FRISE_ORIGINS.Add(new Generic<Pickable>(f));
					} else {
						ACTORS_ORIGINS.Add(new Generic<Pickable>(a));
					}

					return true;
				}
			} else {
				if (ACTORS == null) ACTORS = new CArrayO<Generic<Actor>>();
				if (FRISE == null) FRISE = new CRefList<Frise>();
				if (a != null && !ACTORS.Any(ac => ac?.obj == a) && !FRISE.Any(f => f == a)) {
					if (string.IsNullOrEmpty(name))
						name = a.USERFRIENDLY;

					if (name != null) {
						var newName = name;
						newName = GetNewActorName(newName);
						a.USERFRIENDLY = newName;
					}
					if (a is Frise f) {
						FRISE.Add(f);
					} else {
						ACTORS.Add(new Generic<Actor>(a));
					}

					return true;
				}
			}
			return false;
		}

		string GetNewActorName(string newName) {
			if (FindByName(newName) != null) {
				var basename = newName;
				int i = 1; // New actor names start from name@1 in UbiArt, we do the same

				// Check if name is already in the name@1 format
				const string namePattern = @"^(?<basename>.*)@(?<index>[0-9]+)$";
				var match = Regex.Match(newName, namePattern, RegexOptions.IgnoreCase);
				if (match.Success) {
					var newbasename = match.Groups["basename"].Value;
					if (int.TryParse(match.Groups["index"].Value, out int newIndex) && newIndex > 0 && newIndex < short.MaxValue) {
						i = newIndex;
						basename = newbasename;
					}
				}

				while (FindByName(newName) != null) {
					newName = $"{basename}@{i}";
					i++;
				}
			}
			return newName;
		}

		public void DeletePickable(Pickable pickable) {
			if (pickable is Frise) {
				if (FRISE != null) {
					var matchingActors = FRISE.Where(a => a == pickable).ToList();
					if (matchingActors != null && matchingActors.Any()) {
						foreach (var act in matchingActors)
							FRISE.Remove(act);
					}
				}
				if (FRISE_ORIGINS != null) {
					var matchingActors = FRISE_ORIGINS.Where(a => a.obj == pickable).ToList();
					if (matchingActors != null && matchingActors.Any()) {
						foreach (var act in matchingActors)
							FRISE_ORIGINS.Remove(act);
					}
				}
			}
			if (pickable is Actor) {
				if (ACTORS != null) {
					var matchingActors = ACTORS.Where(a => a.obj == pickable).ToList();
					if (matchingActors != null && matchingActors.Any()) {
						foreach (var act in matchingActors)
							ACTORS.Remove(act);
					}
				}
				if (ACTORS_ORIGINS != null) {
					var matchingActors = ACTORS_ORIGINS.Where(a => a.obj == pickable).ToList();
					if (matchingActors != null && matchingActors.Any()) {
						foreach (var act in matchingActors)
							ACTORS_ORIGINS.Remove(act);
					}
				}
			}
		}


		protected override void OnPreSerialize(CSerializerObject s) {
			base.OnPreSerialize(s);
			Reinit(s.Context, s.Settings);
		}

		Settings previousSettings = null;
		protected virtual void Reinit(Context c, Settings settings) {
			if (previousSettings != null) {
				if (previousSettings.Game != settings.Game) {
					ENGINE_VERSION = 0;
					if(previousSettings.EngineVersion == EngineVersion.RO && settings.EngineVersion == EngineVersion.RL) {
						FRISE = new CRefList<Frise>(FRISE_ORIGINS.Select(f => (Frise)f.obj).ToList());
						ACTORS = new CArrayO<Generic<Actor>>(ACTORS_ORIGINS.Select(a => new Generic<Actor>((Actor)a.obj)).ToArray());
					}
				}
			}
			previousSettings = settings;
		}

		#region Search
		public Pickable FindByName(string name) {
			if(name == null) return null;

			Pickable p = ACTORS?.FirstOrDefault(act => act?.obj?.USERFRIENDLY == name)?.obj;
			if (p == null) p = FRISE?.FirstOrDefault(f => f.USERFRIENDLY == name);
			if (p == null) p = ACTORS_ORIGINS?.FirstOrDefault(act => act?.obj?.USERFRIENDLY == name)?.obj;
			if (p == null) p = FRISE_ORIGINS?.FirstOrDefault(act => act?.obj?.USERFRIENDLY == name)?.obj;

			return p;
		}
		public SearchResult<Pickable> FindPickable(Predicate<Pickable> p, SearchFlags flags = SearchFlags.AllRecursive) {
			if (flags.HasFlag(SearchFlags.Frise)) {
				if (FRISE != null) {
					foreach (var fr in FRISE) {
						if(fr == null) continue;
						if (p(fr)) {
							return new SearchResult<Pickable>() {
								Path = new ObjectPath() { id = fr.USERFRIENDLY },
								Result = fr,
								ContainingScene = this,
								RootScene = this,
							};
						}
					}
				}
				if (FRISE_ORIGINS != null) {
					foreach (var fr in FRISE_ORIGINS) {
						if (fr?.obj == null) continue;
						if (p(fr.obj)) {
							return new SearchResult<Pickable>() {
								Path = new ObjectPath() { id = fr.obj.USERFRIENDLY },
								Result = fr.obj,
								ContainingScene = this,
								RootScene = this,
							};
						}
					}
				}
			}
			if (flags.HasFlag(SearchFlags.Actors)) {
				if (ACTORS != null) {
					foreach (var act in ACTORS) {
						if(act?.obj == null) continue;
						if (p(act.obj)) {
							return new SearchResult<Pickable>() {
								Path = new ObjectPath() { id = act.obj.USERFRIENDLY },
								Result = act.obj,
								ContainingScene = this,
								RootScene = this,
							};
						}
						if (flags.HasFlag(SearchFlags.Recursive) && act.obj is SubSceneActor ssa && ssa?.GetScene() != null) {
							var ss = ssa?.GetScene();
							var ssResult = ss.FindPickable(p, flags);
							if (ssResult != null) {
								List<ObjectPath.Level> levels = new List<ObjectPath.Level>();
								List<SubSceneActor> ssaLevels = new List<SubSceneActor>();
								levels.Add(new ObjectPath.Level() {
									name = ssa.USERFRIENDLY
								});
								ssaLevels.Add(ssa);
								if (ssResult.Path.levels != null) {
									foreach (var lev in ssResult.Path.levels) {
										levels.Add(lev);
									}
								}
								if (ssResult.PathLevels != null) {
									foreach (var lev in ssResult.PathLevels) {
										ssaLevels.Add(lev);
									}
								}
								var newResult = new SearchResult<Pickable>() {
									RootScene = this,
									ContainingScene = ssResult.ContainingScene,
									Path = new ObjectPath() {
										levels = new CListO<ObjectPath.Level>(levels),
										id = ssResult.Path.id
									},
									PathLevels = ssaLevels,
									Result = ssResult.Result
								};

								return newResult;
							}
						}
					}
				}
				if (ACTORS_ORIGINS != null) {
					foreach (var act in ACTORS_ORIGINS) {
						if (act?.obj == null) continue;
						if (p(act.obj)) {
							return new SearchResult<Pickable>() {
								Path = new ObjectPath() { id = act.obj.USERFRIENDLY },
								Result = act.obj,
								ContainingScene = this,
								RootScene = this,
							};
						}
						if (flags.HasFlag(SearchFlags.Recursive) && act.obj is SubSceneActor ssa && ssa?.GetScene() != null) {
							var ss = ssa?.GetScene();
							var ssResult = ss.FindPickable(p, flags);
							if (ssResult != null) {
								List<ObjectPath.Level> levels = new List<ObjectPath.Level>();
								List<SubSceneActor> ssaLevels = new List<SubSceneActor>();
								levels.Add(new ObjectPath.Level() {
									name = ssa.USERFRIENDLY
								});
								ssaLevels.Add(ssa);
								if (ssResult.Path.levels != null) {
									foreach (var lev in ssResult.Path.levels) {
										levels.Add(lev);
									}
								}
								if (ssResult.PathLevels != null) {
									foreach (var lev in ssResult.PathLevels) {
										ssaLevels.Add(lev);
									}
								}
								var newResult = new SearchResult<Pickable>() {
									RootScene = this,
									ContainingScene = ssResult.ContainingScene,
									Path = new ObjectPath() {
										levels = new CListO<ObjectPath.Level>(levels),
										id = ssResult.Path.id
									},
									PathLevels = ssaLevels,
									Result = ssResult.Result
								};

								return newResult;
							}
						}
					}
				}
			}
			return null;
		}
		public SearchResult<Actor> FindActor(Predicate<Actor> a, SearchFlags flags = SearchFlags.Actors | SearchFlags.Recursive) {
			var pickable = FindPickable(p => a(p as Actor), flags & ~SearchFlags.Frise);
			if (pickable != null) {
				return new SearchResult<Actor>() {
					ContainingScene = pickable.ContainingScene,
					RootScene = pickable.RootScene,
					Path = pickable.Path,
					Result = pickable.Result as Actor,
					PathLevels = pickable.PathLevels
				};
			}
			return null;
		}
		public List<SearchResult<Pickable>> FindPickables(Predicate<Pickable> p, SearchFlags flags = SearchFlags.AllRecursive) {
			List<SearchResult<Pickable>> results = new List<SearchResult<Pickable>>();
			if (flags.HasFlag(SearchFlags.Frise)) {
				if (FRISE != null) {
					foreach (var fr in FRISE) {
						if (fr == null) continue;
						if (p(fr)) {
							results.Add(new SearchResult<Pickable>() {
								Path = new ObjectPath() { id = fr.USERFRIENDLY },
								Result = fr,
								ContainingScene = this,
								RootScene = this
							});
						}
					}
				}
				if (FRISE_ORIGINS != null) {
					foreach (var fr in FRISE_ORIGINS) {
						if (fr?.obj == null) continue;
						if (p(fr.obj)) {
							results.Add(new SearchResult<Pickable>() {
								Path = new ObjectPath() { id = fr.obj.USERFRIENDLY },
								Result = fr.obj,
								ContainingScene = this,
								RootScene = this,
							});
						}
					}
				}
			}
			if (flags.HasFlag(SearchFlags.Actors)) {
				if (ACTORS != null) {
					foreach (var act in ACTORS) {
						if (act?.obj == null) continue;
						if (p(act.obj)) {
							results.Add(new SearchResult<Pickable>() {
								Path = new ObjectPath() { id = act.obj.USERFRIENDLY },
								Result = act.obj,
								ContainingScene = this,
								RootScene = this,
							});
						}
						if (flags.HasFlag(SearchFlags.Recursive) && act.obj is SubSceneActor ssa && ssa?.GetScene() != null) {
							var ss = ssa?.GetScene();
							var ssResults = ss.FindPickables(p, flags);
							if (ssResults != null && ssResults.Any()) {
								foreach (var ssResult in ssResults) {
									List<ObjectPath.Level> levels = new List<ObjectPath.Level>();
									List<SubSceneActor> ssaLevels = new List<SubSceneActor>();
									levels.Add(new ObjectPath.Level() {
										name = ssa.USERFRIENDLY
									});
									ssaLevels.Add(ssa);
									if (ssResult.Path.levels != null) {
										foreach (var lev in ssResult.Path.levels) {
											levels.Add(lev);
										}
									}
									if (ssResult.PathLevels != null) {
										foreach (var lev in ssResult.PathLevels) {
											ssaLevels.Add(lev);
										}
									}
									var newResult = new SearchResult<Pickable>() {
										RootScene = this,
										ContainingScene = ssResult.ContainingScene,
										Path = new ObjectPath() {
											levels = new CListO<ObjectPath.Level>(levels),
											id = ssResult.Path.id
										},
										PathLevels = ssaLevels,
										Result = ssResult.Result
									};
									results.Add(newResult);
								}
							}
						}
					}
				}

				if (ACTORS_ORIGINS != null) {
					foreach (var act in ACTORS_ORIGINS) {
						if (act?.obj == null) continue;
						if (p(act.obj)) {
							results.Add(new SearchResult<Pickable>() {
								Path = new ObjectPath() { id = act.obj.USERFRIENDLY },
								Result = act.obj,
								ContainingScene = this,
								RootScene = this,
							});
						}
						if (flags.HasFlag(SearchFlags.Recursive) && act.obj is SubSceneActor ssa && ssa?.GetScene() != null) {
							var ss = ssa?.GetScene();
							var ssResults = ss.FindPickables(p, flags);
							if (ssResults != null && ssResults.Any()) {
								foreach (var ssResult in ssResults) {
									List<ObjectPath.Level> levels = new List<ObjectPath.Level>();
									List<SubSceneActor> ssaLevels = new List<SubSceneActor>();
									levels.Add(new ObjectPath.Level() {
										name = ssa.USERFRIENDLY
									});
									ssaLevels.Add(ssa);
									if (ssResult.Path.levels != null) {
										foreach (var lev in ssResult.Path.levels) {
											levels.Add(lev);
										}
									}
									if (ssResult.PathLevels != null) {
										foreach (var lev in ssResult.PathLevels) {
											ssaLevels.Add(lev);
										}
									}
									var newResult = new SearchResult<Pickable>() {
										RootScene = this,
										ContainingScene = ssResult.ContainingScene,
										Path = new ObjectPath() {
											levels = new CListO<ObjectPath.Level>(levels),
											id = ssResult.Path.id
										},
										PathLevels = ssaLevels,
										Result = ssResult.Result
									};
									results.Add(newResult);
								}
							}
						}
					}
				}
			}
			return results;
		}
		public List<SearchResult<Actor>> FindActors(Predicate<Actor> a, SearchFlags flags = SearchFlags.Actors | SearchFlags.Recursive) {
			List<SearchResult<Actor>> results = new List<SearchResult<Actor>>();
			var pickables = FindPickables(p => a(p as Actor), flags & ~SearchFlags.Frise);
			if (pickables != null) {
				foreach (var pickable in pickables) {
					results.Add(new SearchResult<Actor>() {
						ContainingScene = pickable.ContainingScene,
						RootScene = pickable.RootScene,
						Path = pickable.Path,
						Result = pickable.Result as Actor,
						PathLevels = pickable.PathLevels
					});
				}
			}
			return results;
		}

		public ObjectPath GetRelativePath(Pickable from, Pickable to) {
			var fromPath = FindPickable(p => p == from);
			var toPath = FindPickable(p => p == to);
			var fromLevelsCount = (fromPath.Path.levels?.Count ?? 0);
			var toLevelsCount = (toPath.Path.levels?.Count ?? 0);

			int sameLevels = 0;

			for (int i = 0; i < fromLevelsCount; i++) {
				// We don't need to take into account "parent" levels as FindPickable will not produce those
				if (i >= toLevelsCount || fromPath.Path.levels[i].name != toPath.Path.levels[i].name) {
					break;
				}
				sameLevels++;
			}
			string path = "";
			for (int i = sameLevels; i < fromLevelsCount; i++) {
				path += "..|";
			}
			for (int i = sameLevels; i < toLevelsCount; i++) {
				path += $"{toPath.Path.levels[i].name}|";
			}
			path += toPath.Path.id;
			return new ObjectPath(path);
		}

		[Flags]
		public enum SearchFlags {
			None = 0,
			Actors = 1 << 0,
			Frise = 1 << 1,
			All = Actors | Frise,

			Recursive = 1 << 2,

			AllRecursive = All | Recursive,
		}

		public class SearchResult<T> where T : Pickable {
			public T Result { get; set; }
			public ObjectPath Path { get; set; }
			public Scene RootScene { get; set; }
			public Scene ContainingScene { get; set; }
			public List<SubSceneActor> PathLevels { get; set; }

			public void Delete() => ContainingScene.DeletePickable(Result);
		}
		#endregion
	}
}
