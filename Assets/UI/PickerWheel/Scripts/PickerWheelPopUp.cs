using UnityEngine ;
using UnityEngine.UI ;
using DG.Tweening ;
using UnityEngine.Events ;
using System.Collections.Generic ;

namespace EasyUI.PickerWheelUI {

   public class PickerWheelPopUp : PopupProperties {

      [Header ("References :")]
      [SerializeField] private GameObject linePrefab ;
      [SerializeField] private Transform linesParent ;

      [Space]
      [SerializeField] private Transform PickerWheelTransform ;
      [SerializeField] private Transform wheelCircle ;
      [SerializeField] private GameObject wheelPiecePrefab ;
      [SerializeField] private Transform wheelPiecesParent ;

      [Space]
      [Header ("Sounds :")]
      [SerializeField] private AudioSource audioSource ;
      [SerializeField] private AudioClip tickAudioClip ;
      [SerializeField] [Range (0f, 1f)] private float volume = .5f ;
      [SerializeField] [Range (-3f, 3f)] private float pitch = 1f ;

      [Space]
      [Header ("Picker wheel settings :")]
      [Range (1, 20)] public int spinDuration = 8 ;
      [SerializeField] [Range (.2f, 2f)] private float wheelSize = 1f ;

      [Space]
      [Header ("Picker wheel pieces :")]
      public WheelPiece[] wheelPieces ;
        public List<WheelPiece> pieces = new List<WheelPiece>();
        public Text textUpdate;
        public Text textEnd;
        public GameObject result;

      // Events
      private UnityAction onSpinStartEvent ;
        private UnityAction<WheelPiece> onSpinUpdateEvent;
        private UnityAction<WheelPiece> onSpinEndEvent ;


      private bool _isSpinning = false ;

      public bool IsSpinning { get { return _isSpinning ; } }


      private Vector2 pieceMinSize = new Vector2 (81f, 146f) ;
      private Vector2 pieceMaxSize = new Vector2 (144f, 213f) ;
      private int piecesMin = 2 ;
      private int piecesMax = 12 ;

      private float pieceAngle ;
      private float halfPieceAngle ;
      private float halfPieceAngleWithPaddings ;


      private double accumulatedWeight ;
      private System.Random rand = new System.Random () ;

        int total;

      //private List<int> nonZeroChancesIndices = new List<int> () ;

      private void OnEnable () {
            if (SpinPopUp.instance.currentSpin == 2) total = wheelPieces.Length;
            else total = SpinPopUp.instance.total;
         pieceAngle = 360 / total ;
         halfPieceAngle = pieceAngle / 2f ;
         halfPieceAngleWithPaddings = halfPieceAngle - (halfPieceAngle / 4f) ;

         Generate () ;

            OnSpinUpdate(wheelPiece =>
            {
                textUpdate.text = wheelPiece.Label;
            });

            OnSpinEnd(wheelPiece =>
            {
                result.SetActive(true);
                textEnd.text = wheelPiece.Label;
            });
            //CalculateWeightsAndIndices () ;
            //if (nonZeroChancesIndices.Count == 0)
            //   Debug.LogError ("You can't set all pieces chance to zero") ;


            //SetupAudio () ;

        }

      private void SetupAudio () {
         audioSource.clip = tickAudioClip ;
         audioSource.volume = volume ;
         audioSource.pitch = pitch ;
      }

      private void Generate () {
            //wheelPiecePrefab = InstantiatePiece () ;

            //RectTransform rt = wheelPiecePrefab.transform.GetChild (0).GetComponent <RectTransform> () ;
            //float pieceWidth = Mathf.Lerp (pieceMinSize.x, pieceMaxSize.x, 1f - Mathf.InverseLerp (piecesMin, piecesMax, total)) ;
            //float pieceHeight = Mathf.Lerp (pieceMinSize.y, pieceMaxSize.y, 1f - Mathf.InverseLerp (piecesMin, piecesMax, total)) ;
            //rt.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, pieceWidth) ;
            //rt.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, pieceHeight) ;

            pieces.Clear();
         for (int i = 0; i < total; i++)
            DrawPiece (i) ;

         //Destroy (wheelPiecePrefab) ;
      }

      private void DrawPiece (int index) {
         //WheelPiece piece = wheelPieces [ index ] ;
         Transform pieceTrns = InstantiatePiece ().transform;
            pieceTrns.GetChild(0).GetComponent<Image>().fillAmount = 1f / total;
            pieceTrns.GetChild(0).GetComponent<Image>().color = Random.ColorHSV();
            pieceTrns.GetChild(0).eulerAngles = new Vector3(0, 0, 180f / total - 180f);
            pieces.Add(new WheelPiece());
            //pieceTrns.GetChild (0).GetComponent <Image> ().sprite = piece.Icon ;
            if (SpinPopUp.instance.currentSpin == 2)
         {
                WheelPiece piece = wheelPieces[index];
                pieceTrns.GetChild(1).GetChild(1).GetComponent<Text>().text = piece.Label;
                pieces[index].Label = piece.Label;
         }
         else
         {
                pieceTrns.GetChild(1).GetChild(1).GetComponent<Text>().text = ""  + (index + 1);
                pieces[index].Label = "" + (index + 1);
            }
           
         
         //pieceTrns.GetChild (2).GetComponent <Text> ().text = piece.Amount.ToString () ;

         //Line
         Transform lineTrns = Instantiate (linePrefab, linesParent.position, Quaternion.identity, linesParent).transform ;
         lineTrns.RotateAround (wheelPiecesParent.position, Vector3.back, (pieceAngle * index) + halfPieceAngle) ;

         pieceTrns.RotateAround (wheelPiecesParent.position, Vector3.back, pieceAngle * index) ;
      }

      private GameObject InstantiatePiece () {
         return Instantiate (wheelPiecePrefab, wheelPiecesParent.position, Quaternion.identity, wheelPiecesParent) ;
      }


      public void Spin () {
         if (!_isSpinning) {
            _isSpinning = true ;
            if (onSpinStartEvent != null)
               onSpinStartEvent.Invoke () ;

                //int index = GetRandomPieceIndex () ;
                int index = Random.Range(0, total);
                WheelPiece piece = pieces [ index ] ;

            //if (piece.Chance == 0 && nonZeroChancesIndices.Count != 0) {
            //   index = nonZeroChancesIndices [ Random.Range (0, nonZeroChancesIndices.Count) ] ;
            //   piece = wheelPieces [ index ] ;
            //}

            float angle = -(pieceAngle * index) ;

            float rightOffset = (angle - halfPieceAngleWithPaddings) % 360 ;
            float leftOffset = (angle + halfPieceAngleWithPaddings) % 360 ;

            float randomAngle = Random.Range (leftOffset, rightOffset) ;

            Vector3 targetRotation = Vector3.back * (randomAngle + 2 * 360 * spinDuration) ;

            //float prevAngle = wheelCircle.eulerAngles.z + halfPieceAngle ;
            float prevAngle, currentAngle ;
            prevAngle = currentAngle = wheelCircle.eulerAngles.z ;

            bool isIndicatorOnTheLine = false ;

            wheelCircle
            .DORotate (targetRotation, spinDuration, RotateMode.FastBeyond360)
            .SetEase (Ease.InOutQuart)
            .OnUpdate (() => {
               float diff = Mathf.Abs (prevAngle - currentAngle) ;
               if (diff >= halfPieceAngle) {
                  if (isIndicatorOnTheLine) {
                     //audioSource.PlayOneShot (audioSource.clip) ;
                  }
                  prevAngle = currentAngle ;
                  isIndicatorOnTheLine = !isIndicatorOnTheLine ;
               }
               currentAngle = wheelCircle.eulerAngles.z ;
                int index = (int)((currentAngle + halfPieceAngle) / pieceAngle);
                if (index == total) index = 0;
                WheelPiece pieceUpdate = pieces[index];
                Debug.LogError("=============== " + wheelCircle.eulerAngles.z);
                if (onSpinUpdateEvent != null)
                    onSpinUpdateEvent.Invoke(pieceUpdate);
            })
            .OnComplete (() => {
               _isSpinning = false ;
                if (onSpinEndEvent != null)
                    onSpinEndEvent.Invoke(piece);

            }) ;

         }
      }

      private void FixedUpdate () {

      }

      public void OnSpinStart (UnityAction action) {
         onSpinStartEvent = action ;
      }

        public void OnSpinUpdate(UnityAction<WheelPiece> action)
        {
            onSpinUpdateEvent = action;
        }

        public void OnSpinEnd (UnityAction<WheelPiece> action) {
         onSpinEndEvent = action ;
      }


      private int GetRandomPieceIndex () {
         double r = rand.NextDouble () * accumulatedWeight ;

         //for (int i = 0; i < wheelPieces.Length; i++)
         //   if (wheelPieces [ i ]._weight >= r)
         //      return i ;

         return 0 ;
      }

      private void CalculateWeightsAndIndices () {
         for (int i = 0; i < wheelPieces.Length; i++) {
            WheelPiece piece = wheelPieces [ i ] ;

            //add weights:
            //accumulatedWeight += piece.Chance ;
            //piece._weight = accumulatedWeight ;

            //add index :
            piece.Index = i ;

            //save non zero chance indices:
            //if (piece.Chance > 0)
            //   nonZeroChancesIndices.Add (i) ;
         }
      }




      private void OnValidate () {
         if (PickerWheelTransform != null)
            PickerWheelTransform.localScale = new Vector3 (wheelSize, wheelSize, 1f) ;

         if (wheelPieces.Length > piecesMax || wheelPieces.Length < piecesMin)
            Debug.LogError ("[ PickerWheelwheel ]  pieces length must be between " + piecesMin + " and " + piecesMax) ;
      }

        public new void BtnClose()
        {
            Destroy(gameObject);
        }

        public void BtnCloseResult()
        {
            result.SetActive(false);
        }
   }
}