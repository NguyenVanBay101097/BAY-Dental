import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IrModelListComponent } from './ir-model-list.component';

describe('IrModelListComponent', () => {
  let component: IrModelListComponent;
  let fixture: ComponentFixture<IrModelListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IrModelListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IrModelListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
