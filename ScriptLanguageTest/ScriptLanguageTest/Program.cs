using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

class PredicateBase
{
    public virtual bool IsTrue()
    {
        return true;
    }
}

/// <summary>
/// 哑 谓词
/// </summary>
class DummyPredicateA : PredicateBase
{
    public override bool IsTrue()
    {
        return true;
    }
}

class DummyPredicateB : PredicateBase
{
    public override bool IsTrue()
    {
        return true;
    }
}

class DummyPredicateC : PredicateBase
{
    public override bool IsTrue()
    {
        return true;
    }
}

/// <summary>
/// 与 节点
/// </summary>
class AndNode : PredicateBase
{
    /// <summary>
    /// 参考c语言优先级
    /// </summary>
    public const int m_OperatorPriority = 11;

    public void Init(PredicateBase predicate1, PredicateBase predicate2)
    {
        m_predicate1 = predicate1;
        m_predicate2 = predicate2;
    }

    public override bool IsTrue()
    {
        return m_predicate1.IsTrue() && m_predicate2.IsTrue();
    }

    PredicateBase m_predicate1 = null;
    PredicateBase m_predicate2 = null;
}

/// <summary>
/// 或 节点
/// </summary>
class OrNode : PredicateBase
{
    /// <summary>
    /// 参考c语言优先级
    /// </summary>
    public const int m_OperatorPriority = 12;

    public void Init(PredicateBase predicate1, PredicateBase predicate2)
    {
        m_predicate1 = predicate1;
        m_predicate2 = predicate2;
    }

    public override bool IsTrue()
    {
        return m_predicate1.IsTrue() || m_predicate2.IsTrue();
    }

    PredicateBase m_predicate1 = null;
    PredicateBase m_predicate2 = null;
}

/// <summary>
/// 非 节点
/// </summary>
class NotNode : PredicateBase
{
    /// <summary>
    /// 参考c语言优先级
    /// </summary>
    public const int m_OperatorPriority = 2;

    public void Init(PredicateBase predicate)
    {
        m_predicate = predicate;
    }

    public override bool IsTrue()
    {
        return !m_predicate.IsTrue();
    }

    PredicateBase m_predicate = null;
}

class Parser
{
    PredicateBase m_predicate = null;
    /// <summary>
    /// 操作数堆栈
    /// </summary>
    Stack<PredicateBase> m_operandStack = new Stack<PredicateBase>();
    /// <summary>
    /// 操作符堆栈
    /// </summary>
    Stack<PredicateBase> m_operatorStack = new Stack<PredicateBase>();

    //string m_param = null;
    List<Char> m_param = new List<char>();

    public Parser(string param)
    {
        // todo 错误抛异常
        param = param.Replace(" ", "").ToLower();

        foreach(var iter in param)
        {
            m_param.Add(iter);
        }

        while(m_param.Count > 0)
        {
            string token = GetToken();
            Console.WriteLine("token " + token);
        }
    }

    public bool Eval()
    {
        return m_predicate.IsTrue();
    }

    string GetToken()
    {
        if(m_param.Count == 0)
        {
            throw new Exception("要拆词的序列为空");
        }

        if(IsReserveWord(m_param[0]))
        {
            char token = m_param[0];
            m_param.RemoveAt(0);
            return token.ToString();
        }
        else
        {
            string token = "";
            char oneChar = m_param[0];
            if (IsLegalChar(oneChar))
            {
                while (IsLegalChar(oneChar))
                {
                    token += oneChar;

                    m_param.RemoveAt(0);
                    if (m_param.Count > 0)
                    {
                        oneChar = m_param[0];
                    }
                    else
                    {
                        break;
                    }
                }

                return token;
            }
            else
            {
                throw new Exception("无法识别的token");
            }
        }
    }

    bool IsReserveWord(char word)
    {
        if (word == '('
           || word == ')'
           || word == '&'
           || word == '|'
           || word == '!')
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool IsLegalChar(char oneChar)
    {
        return (oneChar >= '0' && oneChar <= '9')
                || (oneChar >= 'a' && oneChar <= 'z');
    }

    public static PredicateBase CreatePredicate(string type)
    {
        if (type == "a")
        {
            return new DummyPredicateA();
        }
        else if(type == "b")
        {
            return new DummyPredicateB();
        }
        else if(type == "c")
        {
            return new DummyPredicateC();
        }
        else
        {
            return null;
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Parser parser = new Parser("!a & (b | c)");
        //Console.WriteLine("eval " + parser.Eval());
        Console.WriteLine("end");
        Console.ReadLine();
    }
}

