using System.Collections;
using System.Collections.Generic;

/// <summary>
/// switch式が使えない環境で同じような機能を提供します。最後に必ずDefaultを挿入する必要があります。
/// </summary>
/// <typeparam name="U">比較対象の型</typeparam>
/// <typeparam name="T">式の返り値の型</typeparam>
public class MySwitch<U, T> {
    // T:switchで返す値の型
    // U:switchで比較する型
    T res;
    T prevNum;
    dynamic cmp = default(U);
    private bool match = false;

    public MySwitch(U obj) {
        this.cmp = obj;
    }

    /// <summary>
    /// case句
    /// </summary>
    /// <param name="targ">比較のラベル</param>
    /// <param name="num">マッチした場合の値</param>
    /// <returns></returns>
    public MySwitch<U, T> Case(U targ, T num) {
        if(targ.Equals(cmp)) {
            match = true;
            res = num;
        }
        prevNum = num;
        return this;
    }

    /// <summary>
    /// case句のフォールスルー
    /// 当てはまる場合の返り値は連続する条件のはじめに書く必要があります。
    /// </summary>
    /// <param name="targ">比較のラベル</param>
    public MySwitch<U, T> Case(U targ) {
        if(targ.Equals(cmp)) {
            match = true;
            res = prevNum;
        }
        return this;
    }
    /// <summary>
    /// default句
    /// </summary>
    /// <param name="num">デフォルトの値</param>
    /// <returns></returns>
    public T Default(T num) {
        if (match) {
            return res;
        } else {
            return num;
        }
    }
}
