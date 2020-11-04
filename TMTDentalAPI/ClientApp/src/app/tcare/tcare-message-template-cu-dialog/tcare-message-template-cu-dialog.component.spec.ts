import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TcareMessageTemplateCuDialogComponent } from './tcare-message-template-cu-dialog.component';

describe('TcareMessageTemplateCuDialogComponent', () => {
  let component: TcareMessageTemplateCuDialogComponent;
  let fixture: ComponentFixture<TcareMessageTemplateCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TcareMessageTemplateCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TcareMessageTemplateCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
