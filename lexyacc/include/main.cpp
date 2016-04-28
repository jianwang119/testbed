#include "main.hpp"

void Include::clear()
{
    headerfile.clear();
    is_quotation = false;
    is_angle = false;
}

std::ostream& operator<<(std::ostream& s, const Include& i)
{
    if (i.is_angle)
        s << "angle ";
    if (i.is_quotation)
        s << "quotation ";
    s << "head file : [" << i.headerfile << "]";
    return s;
}
