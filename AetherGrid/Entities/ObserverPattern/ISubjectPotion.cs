using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace We_have_doom_at_home.Entities.ObserverPattern;

public interface ISubjectPotion
{
    void AttachPotion(IPotionObserver observer);
    void DetachPotion(IPotionObserver observer);
    void NotifyPotion();
}   
