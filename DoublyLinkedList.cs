using System;
using System.Collections;
using System.Collections.Generic;

namespace OOP_2_2
{
    public class DoublyLinkedList<T> : IEnumerable<T> where T : IComparable<T>
    {
        private Node<T> head;
        private Node<T> tail;
        private int count;
        public int Count
        {
            get
            {
                return count;
            }
        }
        public bool IsEmpty
        {
            get
            {
                if (count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public void Clear()
        {
            Node<T> current = head;
            while (current != null)
            {
                Node<T> temp = current;
                current = current.Next;
                temp.Next = null;
                temp.Previous = null;
            }
            head = null;
            tail = null;
            count = 0;
        }

        public int IndexOf(T data)
        {
            int index = 0;
            Node<T> current = head;
            while (current != null)
            {
                if (current.Value.Equals(data))
                {
                    return index;
                }
                else
                {
                    index++;
                    current = current.Next;
                }
            }
            return -1;
        }
        public int IndexOfNext(T data)
        {
            int index = 0;
            Node<T> current = head;
            while (current != null)
            {
                if (current.Value.Equals(data))
                {
                    return index + 1;
                }
                else
                {
                    index++;
                    current = current.Next;
                }
            }
            return -1;
        }
        public void Add(T data)
        {
            Node<T> node = new Node<T>(data);
            if (head == null)
            {
                head = node;
            }
            else
            {
                tail.Next = node;
                node.Previous = tail;
            }
            tail = node;
            count++;
        }
        public void Add(Node<T> node)
        {
            if (head == null)
            {
                head = node;
            }
            else
            {
                tail.Next = node;
                node.Previous = tail;
            }
            tail = node;
            count++;
        }
        public Node<T> GetFirst()
        {
            return head;
        }
        public Node<T> GetLast()
        {
            return tail;
        }
        public Node<T> GetNextNode(T data)
        {
            Node<T> current = head;
            while (current != null)
            {
                if (current.Value.Equals(data))
                {
                    return current.Next;
                }
                else
                {
                    current = current.Next;
                }
            }
            return null;
        }
        public Node<T> GetPreviuosNode(T data)
        {
            Node<T> current = head;
            while (current != null)
            {
                if (current.Value.Equals(data))
                {
                    return current.Previous;
                }
                else
                {
                    current = current.Next;
                }
            }
            return null;
        }
        public void AddFirst(Node<T> node)
        {
            Node<T> temp = head;
            temp.Previous = node;
            node.Next = temp;
            head = node;
            if (count == 0)
            {
                tail = head;
            }
            count++;
        }
        public Node<T> Max()
        {
            Node<T> max = head;
            Node<T> current = head;
            while (current != null && current.Next != null)
            {
                if (current.Value.CompareTo(current.Next.Value) < 0)
                {
                    max = current.Next;
                }
                current = current.Next;
            }
            return max;
        }
        public Node<T> Min()
        {
            Node<T> min = head;
            Node<T> current = head;
            while (current != null && current.Next != null)
            {
                if (current.Value.CompareTo(current.Next.Value) > 0)
                {
                    min = current.Next;
                }
                current = current.Next;
            }
            return min;
        }
        public bool Remove(T data)
        {
            Node<T> current = head;
            while (current != null)
            {
                if (current.Value.Equals(data))
                {
                    break;
                }
                else
                {
                    current = current.Next;
                }
            }
            if (current != null)
            {
                if (current.Next != null)
                {
                    current.Next.Previous = current.Previous;
                }
                else
                {
                    tail = current.Previous;
                }
                if (current.Previous != null)
                {
                    current.Previous.Next = current.Next;
                }
                else
                {
                    head = current.Next;
                }
                count--;
                return true;
            }
            return false;
        }
        public bool Contains(T data)
        {
            Node<T> current = head;
            while (current != null)
            {
                if (current.Value.Equals(data))
                {
                    return true;
                }
                else
                {
                    current = current.Next;
                }
            }
            return false;
        }
        public bool Edit(T data, T newData)
        {
            Node<T> current = head;
            while (current != null)
            {
                if (current.Value.Equals(data))
                {
                    try
                    {
                        current.Value = newData;
                        return true;
                    }
                    catch
                    {
                        throw new ArgumentException("Был передан неверный аргумент для изменения!");
                    }
                }
                current = current.Next;
            }
            return false;
        }
        public IEnumerator<T> GetEnumerator()
        {
            Node<T> current = head;
            while (current != null)
            {
                yield return current.Value;
                current = current.Next;
            }
        }
        public IEnumerable<T> BackEnumerator()
        {
            Node<T> current = tail;
            while (current != null)
            {
                yield return current.Value;
                current = current.Previous;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this).GetEnumerator();
        }
    }
}
