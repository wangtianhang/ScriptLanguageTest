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

    public virtual string GetPredicate()
    {
        return "";
    }
}

/// <summary>
/// 哑 谓词
/// </summary>
class DummyPredicateA : PredicateBase
{
    public override bool IsTrue()
    {
        Console.WriteLine("a");
        return true;
    }

    public override string GetPredicate()
    {
        return "a";
    }
}

class DummyPredicateB : PredicateBase
{
    public override bool IsTrue()
    {
        Console.WriteLine("b");
        return true;
    }

    public override string GetPredicate()
    {
        return "b";
    }
}

class DummyPredicateC : PredicateBase
{
    public override bool IsTrue()
    {
        Console.WriteLine("c");
        return true;
    }

    public override string GetPredicate()
    {
        return "c";
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
    //public const int m_OperatorPriority = 11;

    public void Init(PredicateBase predicate1, PredicateBase predicate2)
    {
        m_predicate1 = predicate1;
        m_predicate2 = predicate2;
    }

    public override bool IsTrue()
    {
        Console.WriteLine("&");
        return m_predicate1.IsTrue() && m_predicate2.IsTrue();
    }

    public override string GetPredicate()
    {
        return "(" + m_predicate1.GetPredicate() + "&" + m_predicate2.GetPredicate() + ")";
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
    //public const int m_OperatorPriority = 12;

    public void Init(PredicateBase predicate1, PredicateBase predicate2)
    {
        m_predicate1 = predicate1;
        m_predicate2 = predicate2;
    }

    public override bool IsTrue()
    {
        Console.WriteLine("|");
        return m_predicate1.IsTrue() || m_predicate2.IsTrue();
    }

    public override string GetPredicate()
    {
        return "(" + m_predicate1.GetPredicate() + "|" + m_predicate2.GetPredicate() + ")";
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
    //public const int m_OperatorPriority = 2;

    public void Init(PredicateBase predicate)
    {
        m_predicate = predicate;
    }

    public override bool IsTrue()
    {
        Console.WriteLine("!");
        return !m_predicate.IsTrue();
    }

    public override string GetPredicate()
    {
        return "(!"  + m_predicate.GetPredicate() +  ")";
    }

    PredicateBase m_predicate = null;
}

public enum TokenType
{
    None,
    Operand, // 操作数
    Operator, // 操作符
}

class Token
{
    public Token(TokenType type, string token)
    {
        m_type = type;
        m_token = token;
    }
    public TokenType m_type = TokenType.None;
    public string m_token = "";
}

class Parser
{
    //PredicateBase m_predicate = null;
    /// <summary>
    /// 操作数堆栈
    /// </summary>
    Stack<PredicateBase> m_operandStack = new Stack<PredicateBase>();
    /// <summary>
    /// 操作符堆栈
    /// </summary>
    Stack<Token> m_operatorStack = new Stack<Token>();

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
            Token token = GetToken();
            Console.WriteLine("token " + token.m_token + " " + token.m_type);
            if(token.m_type == TokenType.Operand)
            {
                PredicateBase predicate = CreatePredicate(token.m_token);
                if (m_operatorStack.Count > 0
                    && m_operatorStack.Peek().m_token == "!")
                {
                    NotNode notNode = new NotNode();
                    notNode.Init(predicate);
                    m_operandStack.Push(notNode);
                    m_operatorStack.Pop();
                }
                else
                {
                    m_operandStack.Push(predicate);
                }
            }
            else if (token.m_type == TokenType.Operator)
            {
                if (token.m_token == "!")
                {
                    //PredicateBase predicate = m_operandStack.Pop();
                    //NotNode notNode = new NotNode();
                    //notNode.Init(predicate);
                    //m_operandStack.Push(predicate);
                    m_operatorStack.Push(token);
                }
                else if (token.m_token == "&"
                    || token.m_token == "|")
                {
                    if(m_operatorStack.Count > 0)
                    {
                        if (GetPriority(m_operatorStack.Peek().m_token) < GetPriority(token.m_token))
                        {
                            HandleStackTopOperator();
                            m_operatorStack.Push(token);
                        }
                        else
                        {
                            m_operatorStack.Push(token);
                        }
                    }
                    else
                    {
                        m_operatorStack.Push(token);
                    }
                }
                else if(token.m_token == "(")
                {
                    m_operatorStack.Push(token);
                }
                else if (token.m_token == ")")
                {
                    Token cacheOperator = m_operatorStack.Peek();
                    while (cacheOperator.m_token != "(")
                    {
                        HandleStackTopOperator();
                        cacheOperator = m_operatorStack.Peek();
                    }

                    m_operatorStack.Pop();
                }
            }
        }

        if (m_operatorStack.Count != 0)
        {
            HandleStackTopOperator();
        }
    }

    void HandleStackTopOperator()
    {
        Token cacheOperator = m_operatorStack.Pop();
        PredicateBase predicate1 = m_operandStack.Pop();
        PredicateBase predicate2 = m_operandStack.Pop();

        if (cacheOperator.m_token == "&")
        {
            AndNode andNode = new AndNode();
            andNode.Init(predicate1, predicate2);
            m_operandStack.Push(andNode);
        }
        else if (cacheOperator.m_token == "|")
        {
            OrNode orNode = new OrNode();
            orNode.Init(predicate1, predicate2);
            m_operandStack.Push(orNode);
        }
    }

    public bool Eval()
    {
        if (m_operandStack.Count == 1)
        {
            PredicateBase predicate = m_operandStack.Peek();
            Console.WriteLine(predicate.GetPredicate());
            return predicate.IsTrue();
        }
        else
        {
            throw new Exception("操作数堆栈数量大于1");
        }
    }

    int GetPriority(string operatorWord)
    {
        if(operatorWord == "&")
        {
            return 11;
        }
        else if(operatorWord == "|")
        {
            return 12;
        }
        else if (operatorWord == "!")
        {
            return 2;
        }
        else
        {
            return 99;
        }
    }

    Token GetToken()
    {
        if(m_param.Count == 0)
        {
            throw new Exception("要拆词的序列为空");
        }

        if(IsReserveWord(m_param[0]))
        {
            char token = m_param[0];
            m_param.RemoveAt(0);
            return new Token(TokenType.Operator, token.ToString());
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

                return new Token(TokenType.Operand, token);
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

    public static PredicateBase CreatePredicate(string token)
    {
        if (token == "a")
        {
            return new DummyPredicateA();
        }
        else if (token == "b")
        {
            return new DummyPredicateB();
        }
        else if (token == "c")
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
        Console.WriteLine("eval " + parser.Eval());
        Console.WriteLine("end");
        Console.ReadLine();
    }
}

