import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboOrderCreateUpdateComponent } from './labo-order-create-update.component';

describe('LaboOrderCreateUpdateComponent', () => {
  let component: LaboOrderCreateUpdateComponent;
  let fixture: ComponentFixture<LaboOrderCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboOrderCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboOrderCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
