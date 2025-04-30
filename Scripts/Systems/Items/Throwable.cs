using UnityEngine;

public class Throwable:MonoBehaviour{
    public Rigidbody2D rb;
    public SpriteRenderer sr; //seperate go
    private  float y=1; //this is seperate
    private float yVel;

    public GameObject onCollide;
    
    public bool DestroyOnCollide = true;
    public float destroyDelay = 0;
    
    public void Throw(Vector3 pos, float vel,float _yVel){
         rb.velocity =( pos-transform.position ).normalized * vel;
        yVel = _yVel;
        y = 0.2f;
    }
    
    private void FixedUpdate(){
        sr.transform.localPosition = new Vector3(0, y*0.5f, 0);
        yVel+= -11*Time.deltaTime; //grav
        if (y > 0){
            y += yVel * Time.deltaTime;
        }
        
        else{
            Collide();
            if(!DestroyOnCollide)
                Destroy(gameObject); //still kill on fall??
        }
    }
    
    public void OnCollisionEnter2D(Collision2D other){
        Collide();
    }
    
    public virtual void Collide(){
        
        
        //bouncing
        if (y < 0){
            yVel = -yVel * 0.4f;
            y = 0.01f;
            rb.velocity *= 0.8f;

        }
        else{

        }

        if(onCollide)
            Instantiate(onCollide,transform.position /*(transform.position+sr.transform.position)/2*/, Quaternion.identity);
        if(DestroyOnCollide)
            Destroy(gameObject, destroyDelay);
    }

}