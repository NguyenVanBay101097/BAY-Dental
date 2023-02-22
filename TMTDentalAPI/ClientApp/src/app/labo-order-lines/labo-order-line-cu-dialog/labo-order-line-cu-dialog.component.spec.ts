import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboOrderLineCuDialogComponent } from './labo-order-line-cu-dialog.component';

describe('LaboOrderLineCuDialogComponent', () => {
  let component: LaboOrderLineCuDialogComponent;
  let fixture: ComponentFixture<LaboOrderLineCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboOrderLineCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboOrderLineCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
