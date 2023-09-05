using UnityEngine;

public interface IThrowable {
    public void OnThrow(Vector3 targetPoint, AEntity sourceEntity);
}