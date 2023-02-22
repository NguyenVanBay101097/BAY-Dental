import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DotKhamCreateUpdateComponent } from './dot-kham-create-update.component';

describe('DotKhamCreateUpdateComponent', () => {
  let component: DotKhamCreateUpdateComponent;
  let fixture: ComponentFixture<DotKhamCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DotKhamCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DotKhamCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
