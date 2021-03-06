#include "threadqueue.h"

#if 1
template<class T>
class BlockingCollection
{
private:
    threadqueue *queue;

public:
    BlockingCollection()
    {
        queue = new threadqueue;
        thread_queue_init(queue);
    }
    ~BlockingCollection()
    {
        thread_queue_cleanup(queue, 0);
        delete queue;
    }
    int GetSize()
    {
        return thread_queue_length(queue);
    }
    T Take()
    {
        T item;
        TryTake(item);
        return item;
    }
    bool TryTake(T &item)
    {
        threadmsg msg;
        if  (thread_queue_get(queue, NULL, &msg) != 0)
            return false;
        T *data = (T *)msg.data;
        item = *data;
        delete data;
        return true;
    }
    void Add(const T &item)
    {
        thread_queue_add(queue, new T(item), 0);
    }
    void CompleteAdding()
    {
        thread_queue_complete_adding(queue);
    }
};
#else
template<class T>
class BlockingCollection
{
private:
    std::stack<T> collection;
public:
    BlockingCollection()
    {
    }
    ~BlockingCollection()
    {
    }
    int GetSize()
    {
        return collection.size();
    }
    T Take()
    {
        T item = collection.top();
        collection.pop();
        return item;
    }
    bool TryTake(T &item)
    {
        item = collection.top();
        collection.pop();
    }
    void Add(const T &item)
    {
        collection.push(item);
    }
    void CompleteAdding()
    {
    }
};
#endif
