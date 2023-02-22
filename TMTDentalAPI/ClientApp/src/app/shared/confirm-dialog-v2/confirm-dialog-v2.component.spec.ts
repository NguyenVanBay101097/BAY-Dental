import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmDialogV2Component } from './confirm-dialog-v2.component';

describe('ConfirmDialogV2Component', () => {
  let component: ConfirmDialogV2Component;
  let fixture: ComponentFixture<ConfirmDialogV2Component>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfirmDialogV2Component ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfirmDialogV2Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
