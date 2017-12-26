using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AudioController : MonoBehaviour {

    [Inject(Id = "SE_BreakableBlock_Damage")]
    readonly AudioClip seDamage;
    [Inject(Id = "SE_BreakableBlock_Destruction")]
    readonly AudioClip seDestruction;
    [Inject(Id = "SE_Thurst")]
    readonly AudioClip seThurst;
    [Inject(Id = "SE_Thurst_Head")]
    readonly AudioClip seThurstHead;
    [Inject(Id = "SE_Thurst_Tail")]
    readonly AudioClip seThurstTail;
    [Inject(Id = "SE_Hit")]
    readonly AudioClip seHit;

    [Inject(Id = "SE_Bounce")]
    readonly AudioClip seBounce;
    [Inject(Id = "SE_Connect")]
    readonly AudioClip seConnect;
    [Inject(Id = "SE_Shot")]
    readonly AudioClip seShot;
    [Inject(Id = "SE_Shot2")]
    readonly AudioClip seShot2;

    [Inject(Id = "SE_Start")]
    readonly AudioClip seStart;
    [Inject(Id = "SE_Goal")]
    readonly AudioClip seGoal;
    [Inject(Id = "SE_Select")]
    readonly AudioClip seSelect;

    private AudioSource[] audioSources;

    enum SoundEffectLayer : int
    {
        DamageDestruction = 0,
        Hit = 1,
        Thurst = 2
    }

    [Inject]
    void Construct()
    {
        this.audioSources = this.GetComponents<AudioSource>();
    }

    public void playThurst()
    {
        var src = this.audioSources[(int)SoundEffectLayer.Thurst];
        src.PlayOneShot(this.seThurst);
    }
    public void playThurstHead()
    {
        var src = this.audioSources[(int)SoundEffectLayer.Thurst];
        src.PlayOneShot(this.seThurstHead);
    }
    public void playThurstTail()
    {
        var src = this.audioSources[(int)SoundEffectLayer.Thurst];
        src.PlayOneShot(this.seThurstTail);
    }
    public void playDamage()
    {
        //        this.audioSources[(int)SoundEffectLayer.DamageDestruction].PlayOneShot(this.seDamage);
        this.audioSources[(int)SoundEffectLayer.Hit].PlayOneShot(this.seHit);

    }
    public void playDestruction()
    {
        this.audioSources[(int)SoundEffectLayer.DamageDestruction].PlayOneShot(this.seDestruction);
    }
    public void playHit()
    {
        this.audioSources[(int)SoundEffectLayer.Hit].PlayOneShot(this.seHit);
    }
    public void playBounce()
    {
        this.audioSources[(int)SoundEffectLayer.Hit].PlayOneShot(this.seBounce);
    }
    public void playConnect()
    {
        this.audioSources[(int)SoundEffectLayer.Hit].PlayOneShot(this.seShot);
    }
    public void playShot()
    {
        this.audioSources[(int)SoundEffectLayer.Hit].PlayOneShot(this.seShot2);
    }
    public void playShot2()
    {
        this.audioSources[(int)SoundEffectLayer.Hit].PlayOneShot(this.seDestruction);
    }
    public void playStart()
    {
        this.audioSources[(int)SoundEffectLayer.Hit].PlayOneShot(this.seStart);
    }
    public void playGoal()
    {
        this.audioSources[(int)SoundEffectLayer.Hit].PlayOneShot(this.seGoal);
    }
    public void playSelect()
    {
        this.audioSources[(int)SoundEffectLayer.Hit].PlayOneShot(this.seSelect);
    }
}
