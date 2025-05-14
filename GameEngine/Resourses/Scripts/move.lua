function script.callbacks.update(delta)
	if script.is_key_down "s" then
		script.entity.Transform.PositionY = script.entity.Transform.PositionY - 10 * delta;
	end
	if script.is_key_down "w" then
		script.entity.Transform.PositionY = script.entity.Transform.PositionY + 10 * delta;
	end
	if script.is_key_down "a" then
		script.entity.Transform.PositionX = script.entity.Transform.PositionX - 10 * delta;
	end
	if script.is_key_down "d" then
		script.entity.Transform.PositionX = script.entity.Transform.PositionX + 10 * delta;
	end
end

function script.callbacks.destroy()

end

function script.callbacks.start()
	-- script.entity.Transform.RotationZ = 45;
	-- script.entity.Mass = 45;

end