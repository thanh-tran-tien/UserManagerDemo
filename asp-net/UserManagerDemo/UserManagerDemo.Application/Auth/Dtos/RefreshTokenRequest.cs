﻿namespace UserManagerDemo.Application.Auth.Dtos;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = default!;
}