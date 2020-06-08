﻿using UnityEngine;
using UnityEngine.U2D;

public class SecondarySystem : MonoBehaviour
{
	#region CONFIGURATION
#pragma warning disable 0649
	[Space]
	[Header("ENERGY")]
	[Tooltip("Check it if this organ need energy.")]
	[SerializeField] bool energyNeeded;
	[ConditionalHide("energyNeeded", true)]
	[Tooltip("Time before being empty energy stock when full and not currelty filling.")]
	[SerializeField] float maxEnergy = 15f;
	[ConditionalHide("energyNeeded", true)]
	[Tooltip("Use this parameters to set at which energy capacity this system start.")]
	[SerializeField] float startEnergy = 10f;
	[ConditionalHide("energyNeeded", true)]
	[Tooltip("Assign a lever to this parameter to assiocate it with energy filling boolean.")]
	[SerializeField] LeverScript energyLever;
	[ConditionalHide("energyNeeded", true)]
	[Tooltip("Assign a pipe to this parameter to assiocate it with energy filling boolean.")]
	[SerializeField] GameObject energyPipe;
	/*[ConditionalHide("energyNeeded", true)]
	[Tooltip("Check it if this system energy pipe starts open.")]
	[SerializeField] bool energyStartsOpen;*/
	[Space]
	[Header("OXYGEN")]
	[Tooltip("Check it if this organ need oxygen.")]
	[SerializeField] bool oxygenNeeded;
	[ConditionalHide("oxygenNeeded", true)]
	[Tooltip("Time before being empty oxygen stock when full and not currelty filling.")]
	[SerializeField] float maxOxygen = 15f;
	[ConditionalHide("oxygenNeeded", true)]
	[Tooltip("Use this parameters to set at which oxygen capacity this system start.")]
	[SerializeField] float startOxygen = 10f;
	[ConditionalHide("oxygenNeeded", true)]
	[Tooltip("Assign a lever to this parameter to assiocate it with oxygen filling boolean.")]
	[SerializeField] LeverScript oxygenLever;
	[ConditionalHide("oxygenNeeded", true)]
	[Tooltip("Assign a pipe to this parameter to assiocate it with oxygen filling boolean.")]
	[SerializeField] GameObject oxygenPipe;
	/*[ConditionalHide("oxygenNeeded", true)]
	[Tooltip("Check it if this system oxygen pipe starts open.")]
	[SerializeField] bool oxygenStartsOpen;*/
	[Space]
	[Header("⚠ DON'T TOUCH BELOW ⚠")]
	[Tooltip("Associated energy filling renderer.")]
	[SerializeField] SpriteRenderer energyGaugeRenderer;
	[Tooltip("Associated oxygen filling renderer.")]
	[SerializeField] SpriteRenderer oxygenGaugeRenderer;
#pragma warning restore 0649
	#endregion
	[Header("Components")]
	public Animator animator;
	[Header("Variables")]
	public float currentEnergy;
	public float currentOxygen;
	public bool fillingEnergy;
	public bool fillingOxygen;
	public bool onActivity;
	SpriteShapeController controllerEnergyPipe;
	SpriteShapeController controllerOxygenPipe;
	SpriteShapeRenderer rendererEnergyPipe;
	SpriteShapeRenderer rendererOxygenPipe;
	Material energyFillingMaterial;
	Material oxygenFillingMaterial;

	private void Awake()
	{
		if (!energyNeeded && !oxygenNeeded)
			Debug.LogWarning("Atleast one ressource must be needed by this system : " + name);
		if (energyLever)
		{
			if (energyLever.doublePipeLever)
			{
				energyLever.doubleAssociatedSecondary.Add(this);
				int index = energyLever.doubleAssociatedSecondary.Count - 1;
				energyLever.doubleAssociatedRessources[index] = LeverScript.RessourcesType.energy;
			}
			else
			{
				energyLever.associatedSecondarySystem = this;
				energyLever.associatedRessource = LeverScript.RessourcesType.energy;
			}
		}
		if (oxygenLever)
		{
			if (oxygenLever.doublePipeLever)
			{
				oxygenLever.doubleAssociatedSecondary.Add(this);
				int index = energyLever.doubleAssociatedSecondary.Count - 1;
				oxygenLever.doubleAssociatedRessources[index] = LeverScript.RessourcesType.oxygen;
			}
			else
			{
				oxygenLever.associatedSecondarySystem = this;
				oxygenLever.associatedRessource = LeverScript.RessourcesType.oxygen;
			}
			
		}
		currentEnergy = startEnergy;
		currentOxygen = startOxygen;
		if (energyGaugeRenderer)
		{
			energyFillingMaterial = energyGaugeRenderer.material;
			energyFillingMaterial.SetFloat("Height", currentEnergy / maxEnergy);
		}
			
		if (oxygenGaugeRenderer)
		{
			oxygenFillingMaterial = oxygenGaugeRenderer.material;
			oxygenFillingMaterial.SetFloat("Height", currentOxygen / maxOxygen);
		}
		if (!energyNeeded)
			energyGaugeRenderer.enabled = false;
		if (!oxygenNeeded)
			oxygenGaugeRenderer.enabled = false;
	}

	private void Start()
	{
		if (energyPipe)
		{
			controllerEnergyPipe = energyPipe.GetComponent<SpriteShapeController>();
			rendererEnergyPipe = energyPipe.GetComponent<SpriteShapeRenderer>();
			//if (!energyStartsOpen)
			//{
				for (int i = 0; i < controllerEnergyPipe.spline.GetPointCount(); i++)
					controllerEnergyPipe.spline.SetHeight(i, GameManager.instance.pipeCloseHeight);
				rendererEnergyPipe.color = GameManager.instance.energyPipeCloseColor;
			//}
		}
		if (oxygenPipe)
		{
			controllerOxygenPipe = oxygenPipe.GetComponent<SpriteShapeController>();
			rendererOxygenPipe = oxygenPipe.GetComponent<SpriteShapeRenderer>();
			//if (!oxygenStartsOpen)
			//{
				for (int i = 0; i < controllerOxygenPipe.spline.GetPointCount(); i++)
					controllerOxygenPipe.spline.SetHeight(i, GameManager.instance.pipeCloseHeight);
				rendererOxygenPipe.color = GameManager.instance.oxygenPipeCloseColor;
			//}
		}
		//if (energyStartsOpen && energyLever)
		//{
		//	if (energyLever.doublePipeLever)
		//	{
		//		fillingEnergy = !fillingEnergy;
		//		SwitchEnergyPipe();
		//	}
		//	else
		//		energyLever.Switch();
		//}

		//if (oxygenStartsOpen && oxygenLever)
		//{
		//	if (oxygenLever.doublePipeLever)
		//	{
		//		fillingOxygen = !fillingOxygen;
		//		SwitchOxygenPipe();
		//	}
		//	else
		//		oxygenLever.Switch();
		//}
		if (energyNeeded)
		{
			if (!energyLever)
				Debug.LogError("Please assign an energy lever to this secondary system : " + name);
			if(!energyPipe)
				Debug.LogError("Please assign an energy pipe to this secondary system : " + name);
		}
		if (oxygenNeeded)
		{
			if (!oxygenLever)
				Debug.LogError("Please assign an oxygen lever to this secondary system : " + name);
			if (!oxygenPipe)
				Debug.LogError("Please assign an oxygen pipe to this secondary system : " + name);
		}
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		if (onActivity)
		{
			if (energyNeeded)
			{
				if (fillingEnergy)
					FillingEnergy();
				else
					EmptyingEnergy();
				energyFillingMaterial.SetFloat("Height", currentEnergy / maxEnergy);
			}
			if (oxygenNeeded)
			{
				if (fillingOxygen)
					FillingOxygen();
				else
					EmptyingOxygen();
				oxygenFillingMaterial.SetFloat("Height", currentOxygen / maxOxygen);
			}
			CheckStopActivity();
		}
	}

	public void SwitchEnergyPipe()
	{
		if (fillingEnergy)
		{
			for (int i = 0; i < controllerEnergyPipe.spline.GetPointCount(); i++)
				controllerEnergyPipe.spline.SetHeight(i, 1);
			rendererEnergyPipe.color = GameManager.instance.energyPipeOpenColor;
		}
		else
		{
			for (int i = 0; i < controllerEnergyPipe.spline.GetPointCount(); i++)
				controllerEnergyPipe.spline.SetHeight(i, GameManager.instance.pipeCloseHeight);
			rendererEnergyPipe.color = GameManager.instance.energyPipeCloseColor;
		}
	}

	public void SwitchOxygenPipe()
	{
		if (fillingOxygen)
		{
			for (int i = 0; i < controllerOxygenPipe.spline.GetPointCount(); i++)
				controllerOxygenPipe.spline.SetHeight(i, 1);
			rendererOxygenPipe.color = GameManager.instance.oxygenPipeOpenColor;
		}
		else
		{
			for (int i = 0; i < controllerOxygenPipe.spline.GetPointCount(); i++)
				controllerOxygenPipe.spline.SetHeight(i, GameManager.instance.pipeCloseHeight);
			rendererOxygenPipe.color = GameManager.instance.oxygenPipeCloseColor;
		}
	}

	void FillingEnergy()
	{
		if (StomachManager.instance.Emptying(Time.deltaTime))
		{
			if (currentEnergy + Time.deltaTime >= maxEnergy)
				currentEnergy = maxEnergy;
			else
				currentEnergy += Time.deltaTime;
		}
		else
			EmptyingEnergy();
	}

	void EmptyingEnergy()
	{
		if (currentEnergy - Time.deltaTime >= 0)
			currentEnergy -= Time.deltaTime;
		else
			currentEnergy = 0;
	}

	void FillingOxygen()
	{
		if (LungsManager.instance.Emptying(Time.deltaTime))
		{
			if (currentOxygen + Time.deltaTime >= maxOxygen)
				currentOxygen = maxOxygen;
			else
				currentOxygen += Time.deltaTime;
		}
		else
			EmptyingOxygen();
	}

	void EmptyingOxygen()
	{
		if (currentOxygen - Time.deltaTime >= 0)
			currentOxygen -= Time.deltaTime;
		else
			currentOxygen = 0;
	}

	void CheckStopActivity()
	{
		if(energyNeeded && oxygenNeeded)
		{
			if (currentEnergy / maxEnergy >= 1 && currentOxygen / maxOxygen >= 1)
				StopActivity();
		}
		else if (energyNeeded)
		{
			if (currentEnergy / maxEnergy >= 1)
				StopActivity();
		}
		else if (oxygenNeeded)
		{
			if (currentOxygen / maxOxygen >= 1)
				StopActivity();
		}
	}

	void StopActivity()
	{
		onActivity = false;
		SecondarySystemsManager.instance.secondarySystems.Add(this);
		animator.SetBool("OnActivity", false);
	}
}