using UnityEngine;

public class Throwable:MonoBehaviour{
    public Rigidbody2D rb;
    public SpriteRenderer sr; //seperate go
    private  float y=1; //this is seperate
    private float yVel;

    public GameObject onCollide;
    
    public void Throw(Vector3 pos, float vel,float _yVel){
         rb.velocity =( pos-transform.position ).normalized * vel;
        yVel = _yVel;
        y = 1;
    }
    
    private void FixedUpdate(){
        sr.transform.localPosition = new Vector3(0, y, 0);
        yVel+= -10*Time.deltaTime; //grav
        if (y > 0){
            y += yVel * Time.deltaTime;
        }
        else{
            Collide();
        }
    }
    
    public void OnCollisionEnter2D(Collision2D other){
        Collide();
    }
    
    public virtual void Collide(){
        Instantiate(onCollide, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}