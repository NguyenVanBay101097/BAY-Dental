import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboFinishLineCuDialogComponent } from './labo-finish-line-cu-dialog.component';

describe('LaboFinishLineCuDialogComponent', () => {
  let component: LaboFinishLineCuDialogComponent;
  let fixture: ComponentFixture<LaboFinishLineCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboFinishLineCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboFinishLineCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
