using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagerDemo.Application.Users.Dtos;

public class ReadUserDto : UserDto
{
    public Guid Id { get; set; }
}
