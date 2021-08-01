package com.rfernandon.exceptionhandler.model;

import lombok.*;

import java.io.Serializable;

@Getter
@AllArgsConstructor
@NoArgsConstructor
public class ErrorDetails implements Serializable {

	private String code;
	private String message;
}
