import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IrModelCuDialogComponent } from './ir-model-cu-dialog.component';

describe('IrModelCuDialogComponent', () => {
  let component: IrModelCuDialogComponent;
  let fixture: ComponentFixture<IrModelCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IrModelCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IrModelCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
