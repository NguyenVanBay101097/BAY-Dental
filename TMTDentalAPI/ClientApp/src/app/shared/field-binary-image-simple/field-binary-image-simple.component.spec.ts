import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FieldBinaryImageSimpleComponent } from './field-binary-image-simple.component';

describe('FieldBinaryImageSimpleComponent', () => {
  let component: FieldBinaryImageSimpleComponent;
  let fixture: ComponentFixture<FieldBinaryImageSimpleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FieldBinaryImageSimpleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FieldBinaryImageSimpleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
