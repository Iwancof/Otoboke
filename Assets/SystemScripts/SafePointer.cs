using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SafePointer {
    public class SafePointer<T> {
        public delegate T Deref(); //Dereference つまり値の取得
        public delegate void Indir(T a); //Indirection つまり値の書き込み
        public bool isnull { get; private set; } = true; //値がnullかどうか

        private Deref _deref;
        private Indir _indir;

        public Deref deref {
            get {
                nullcheck();
                return _deref;
            }
            private set { _deref = value; }
        }
        public Indir indir {
            get {
                nullcheck();
                return _indir;
            }
            private set {
                _indir = value;
            }
        }

        public SafePointer(Deref d, Indir i) {
            deref = d;
            indir = i;

            isnull = false;
        }
        public SafePointer() { //値がまたないけどのポインタを作りたいときのコンストラクタ
            isnull = true;
        }

        private void nullcheck() {
            if (isnull) throw new NullReferenceException("値が空です");
        }
    }

    public class SafeEntity<T> { //ポインタを返せる実体
        public T value;

        public SafeEntity(T x) {
            this.value = x;
        }

        public SafePointer<T> getrf() => //ポインタの取得(GET ReFerence)。C/C++でいう "&" 演算子
            new SafePointer<T>(
                () => this.value, //読みだし
                (T a) => this.value = a //書き込み
            );

        public void print() {
            Console.WriteLine($"value = {value}.");
        }
    }
}