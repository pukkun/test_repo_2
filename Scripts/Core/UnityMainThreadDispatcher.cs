/*
Copyright 2015 Pim de Witte All Rights Reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// Author: Pim de Witte (pimdewitte.com) and contributors
/// <summary>
/// A thread-safe class which holds a queue with actions to execute on the next Update() method. It can be used to make calls to the main thread for
/// things such as UI Manipulation in Unity. It was developed for use in combination with the Firebase Unity plugin, which uses separate threads for event handling
/// </summary>
public class UnityMainThreadDispatcher : Singleton<UnityMainThreadDispatcher> {
	protected override void Awake()
	{
		base.Awake();
		Instance.Init();// chay de create instance truoc
	}
    public void Init()
    {
        

    }
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

	public void Update()
    { 
        lock (_executionQueue) {
			while (_executionQueue.Count > 0) {
                Action action = _executionQueue.Dequeue();
                action.Invoke();
			}
		}
	}

	/// <summary>
	/// Locks the queue and adds the IEnumerator to the queue
	/// </summary>
	/// <param name="action">IEnumerator function that will be executed from the main thread.</param>
	public void Enqueue(IEnumerator action) {
		lock (_executionQueue) {
			_executionQueue.Enqueue (() => {
				StartCoroutine (action);
			});
		}
	}
    public void Dequeue(IEnumerator action)
    {
        _executionQueue.Clear();
    }

    /// <summary>
    /// Locks the queue and adds the Action to the queue
    /// </summary>
    /// <param name="action">function that will be executed from the main thread.</param>
    public void Enqueue(Action action)
	{
		Enqueue(ActionWrapper(action));
	}
	IEnumerator ActionWrapper(Action a)
	{
		a();
		yield return null;
	}



}
