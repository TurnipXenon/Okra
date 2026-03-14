using System;
using System.Collections.Generic;
using Okra.Core.Rhythm.Chart;

namespace Okra.Core.Rhythm.Input;

// todo(turnip): refactor code to remove duplicates
public class KeyboardDirectionInput(List<KeyboardSingularInput> singularInputs) : ISingularInput
{
    private const string SlideKeyCode = "SLIDE_KEYCODE";
    private IBeat? _claimingBeat;
    private List<KeyboardSingularInput> _singularInputs = singularInputs;
    private float _direction;

    public bool IsValidDirection()
    {
        return true;
    }

    // todo(turnip): this might depend on our simulator
    public bool IsDirectionSensitive() => false;

    public float GetDirection()
    {
        return _direction;
    }

    public string GetInputCode()
    {
        return SlideKeyCode;
    }

    // todo(turnip): we release claiming beat only if we release the singular beats
    // this is auto released when we dont have singular inputs
    public IBeat? GetClaimingChannel(IBeat contextualBeat) => _claimingBeat;

    public bool ClaimOnStart(IBeat claimingBeat)
    {
        // todo(turnip): consider invalid directions??
        if (_claimingBeat != null || _singularInputState != SingularInputState.Started)
        {
            return false;
        }

        _claimingBeat = claimingBeat;
        return true;
    }

    // todo(turnip): might be released when listening to a singular input that was claimed
    public void ReleaseInput()
    {
        _claimingBeat = null;
    }

    public InputSource GetSource()
    {
        return InputSource.Player;
    }

    public RhythmActionType GetRhythmActionType()
    {
        return RhythmActionType.Directional;
    }

    private SingularInputState _singularInputState = SingularInputState.Free;

    public void Activate()
    {
        _singularInputState = _singularInputState switch
        {
            SingularInputState.Free => SingularInputState.Started,
            SingularInputState.Started or SingularInputState.Held => SingularInputState.Held,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public SingularInputState GetState()
    {
        return _singularInputState;
    }

    public void Release()
    {
        _singularInputState = SingularInputState.Free;
        _claimingBeat?.OnInputRelease();
        _claimingBeat = null;
    }

    public IRhythmInput ActSingle()
    {
        throw new NotImplementedException();
    }
}
