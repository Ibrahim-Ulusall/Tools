﻿using Castle.DynamicProxy;

namespace Core.Utilities.Interceptors;
public abstract class MethodInterception : MethodInterceptionBaseAttribute
{
    protected virtual void OnBefore(IInvocation invocation) { }
    protected virtual void OnAfter(IInvocation invocation) { }
    protected virtual void OnSuccess(IInvocation invocation) { }
    protected virtual void OnException(IInvocation invocation, Exception exception) { }

    public override void Intercept(IInvocation invocation)
    {
        bool isSuccess = true;
        OnBefore(invocation);
        try
        {
            invocation.Proceed();
        }
        catch (Exception exception)
        {
            isSuccess = false;
            OnException(invocation, exception);
            throw;
        }
        finally
        {
            if(isSuccess)
                OnSuccess(invocation);
        }
        OnAfter(invocation);
    }
}
